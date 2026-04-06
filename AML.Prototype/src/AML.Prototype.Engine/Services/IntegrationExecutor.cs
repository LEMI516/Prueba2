using AML.Prototype.Contracts.Models;
using AML.Prototype.Engine.Abstractions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;

namespace AML.Prototype.Engine.Services;

public sealed class IntegrationExecutor(
    HttpClient httpClient,
    IIntegrationDefinitionStore definitionStore,
    ILogger<IntegrationExecutor> logger) : IIntegrationExecutor
{
    public async Task<IntegrationRunResult> RunAsync(
        RunIntegrationRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        if (string.IsNullOrWhiteSpace(request.IntegrationKey))
        {
            return Fail(request.IntegrationKey, "El campo integrationKey es obligatorio.");
        }

        var definition = definitionStore.GetByKey(request.IntegrationKey);
        if (definition is null)
        {
            return Fail(request.IntegrationKey, $"No existe integración con key '{request.IntegrationKey}'.");
        }

        var resolvedPath = ResolvePath(definition.Path, request.PathParameters);
        var queryString = BuildQueryString(request.QueryParameters);
        var requestUrl = $"{definition.BaseUrl}{resolvedPath}{queryString}";

        using var httpRequest = new HttpRequestMessage(ToHttpMethod(definition.Method), requestUrl);
        ApplyHeaders(httpRequest, definition.DefaultHeaders);
        ApplyHeaders(httpRequest, request.Headers);

        if (ShouldSendBody(definition.Method) && request.Payload.Count > 0)
        {
            httpRequest.Content = JsonContent.Create(request.Payload);
        }

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(definition.TimeoutSeconds));

        try
        {
            logger.LogInformation(
                "Ejecutando integración '{IntegrationKey}' hacia {RequestUrl}",
                definition.Key,
                requestUrl);

            using var response = await httpClient.SendAsync(httpRequest, timeoutCts.Token);
            var body = await response.Content.ReadAsStringAsync(timeoutCts.Token);
            stopwatch.Stop();

            return new IntegrationRunResult
            {
                IntegrationKey = definition.Key,
                Success = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                RequestUrl = requestUrl,
                DurationMs = stopwatch.ElapsedMilliseconds,
                ResponseBody = body,
                ErrorMessage = response.IsSuccessStatusCode ? null : "La API respondió con estado no exitoso."
            };
        }
        catch (TaskCanceledException ex)
        {
            stopwatch.Stop();
            logger.LogWarning(ex, "Timeout ejecutando integración '{IntegrationKey}'", definition.Key);
            return new IntegrationRunResult
            {
                IntegrationKey = definition.Key,
                Success = false,
                RequestUrl = requestUrl,
                DurationMs = stopwatch.ElapsedMilliseconds,
                ErrorMessage = $"Timeout al ejecutar integración (>{definition.TimeoutSeconds}s)."
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "Error ejecutando integración '{IntegrationKey}'", definition.Key);
            return new IntegrationRunResult
            {
                IntegrationKey = definition.Key,
                Success = false,
                RequestUrl = requestUrl,
                DurationMs = stopwatch.ElapsedMilliseconds,
                ErrorMessage = ex.Message
            };
        }
    }

    private static IntegrationRunResult Fail(string integrationKey, string errorMessage)
    {
        return new IntegrationRunResult
        {
            IntegrationKey = integrationKey,
            Success = false,
            RequestUrl = string.Empty,
            DurationMs = 0,
            ErrorMessage = errorMessage
        };
    }

    private static void ApplyHeaders(HttpRequestMessage request, IDictionary<string, string> headers)
    {
        foreach (var (key, value) in headers)
        {
            request.Headers.Remove(key);
            request.Headers.TryAddWithoutValidation(key, value);
        }
    }

    private static string ResolvePath(string path, IDictionary<string, string> pathParameters)
    {
        var resolved = path;
        foreach (var (key, value) in pathParameters)
        {
            resolved = resolved.Replace($"{{{key}}}", Uri.EscapeDataString(value), StringComparison.OrdinalIgnoreCase);
        }

        return resolved.StartsWith('/') ? resolved : $"/{resolved}";
    }

    private static string BuildQueryString(IDictionary<string, string> query)
    {
        if (query.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder("?");
        foreach (var (key, value) in query)
        {
            if (sb.Length > 1)
            {
                sb.Append('&');
            }

            sb.Append(Uri.EscapeDataString(key))
                .Append('=')
                .Append(Uri.EscapeDataString(value));
        }

        return sb.ToString();
    }

    private static HttpMethod ToHttpMethod(PrototypeHttpMethod method)
    {
        return method switch
        {
            PrototypeHttpMethod.Get => HttpMethod.Get,
            PrototypeHttpMethod.Post => HttpMethod.Post,
            PrototypeHttpMethod.Put => HttpMethod.Put,
            PrototypeHttpMethod.Patch => HttpMethod.Patch,
            PrototypeHttpMethod.Delete => HttpMethod.Delete,
            _ => HttpMethod.Post
        };
    }

    private static bool ShouldSendBody(PrototypeHttpMethod method)
    {
        return method is PrototypeHttpMethod.Post or PrototypeHttpMethod.Put or PrototypeHttpMethod.Patch;
    }
}
