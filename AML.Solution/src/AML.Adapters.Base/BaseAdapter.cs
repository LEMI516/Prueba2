using AML.Core.Contracts.Adapters;
using AML.Core.Entities;
using AML.Core.Enums;
using AML.Core.Models.Requests;
using AML.Core.Models.Responses;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AML.Adapters.Base;

public abstract class BaseAdapter(HttpClient httpClient) : IServiceAdapter
{
    public abstract string Name { get; }

    public virtual async Task<ServiceResponse> ExecuteAsync(
        ClientService service,
        ServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        if (service.Endpoint is null)
        {
            return new ServiceResponse
            {
                CorrelationId = request.CorrelationId,
                Success = false,
                Message = "La configuración del endpoint no existe."
            };
        }

        using var httpRequest = new HttpRequestMessage(
            ToHttpMethod(service.Endpoint.HttpMethod),
            service.Endpoint.Url);

        ApplyAuthentication(httpRequest, service.Auth);
        ApplyHeaders(httpRequest, service.Headers);

        if (httpRequest.Method != HttpMethod.Get && httpRequest.Method != HttpMethod.Delete)
        {
            var payload = JsonSerializer.Serialize(request.Parameters);
            httpRequest.Content = new StringContent(payload, Encoding.UTF8, "application/json");
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(service.Endpoint.TimeoutSeconds));

        var response = await httpClient.SendAsync(httpRequest, cts.Token);
        var body = await response.Content.ReadAsStringAsync(cts.Token);

        var data = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["raw"] = body
        };

        return new ServiceResponse
        {
            CorrelationId = request.CorrelationId,
            Success = response.IsSuccessStatusCode,
            StatusCode = (int)response.StatusCode,
            Message = response.IsSuccessStatusCode ? "OK" : "Error en servicio externo",
            Data = data
        };
    }

    protected static HttpMethod ResolveHttpMethod(HttpMethodType methodType) => ToHttpMethod(methodType);

    protected static void ApplyAuth(HttpRequestMessage request, ServiceAuth? auth) => ApplyAuthentication(request, auth);

    protected static void ApplyCustomHeaders(HttpRequestMessage request, IEnumerable<ServiceHeader> headers) => ApplyHeaders(request, headers);

    private static void ApplyHeaders(HttpRequestMessage request, IEnumerable<ServiceHeader> headers)
    {
        foreach (var header in headers)
        {
            request.Headers.TryAddWithoutValidation(header.HeaderKey, header.HeaderValue);
        }
    }

    private static void ApplyAuthentication(HttpRequestMessage request, ServiceAuth? auth)
    {
        if (auth is null || auth.AuthType == AuthType.None)
        {
            return;
        }

        if (auth.AuthType == AuthType.ApiKey && !string.IsNullOrWhiteSpace(auth.ApiKeyHeaderName))
        {
            request.Headers.TryAddWithoutValidation(auth.ApiKeyHeaderName, auth.EncryptedApiKey);
            return;
        }

        if (auth.AuthType == AuthType.Basic &&
            !string.IsNullOrWhiteSpace(auth.EncryptedUsername) &&
            !string.IsNullOrWhiteSpace(auth.EncryptedPassword))
        {
            var raw = $"{auth.EncryptedUsername}:{auth.EncryptedPassword}";
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64);
        }
    }

    private static HttpMethod ToHttpMethod(HttpMethodType methodType)
    {
        return methodType switch
        {
            HttpMethodType.Get => HttpMethod.Get,
            HttpMethodType.Post => HttpMethod.Post,
            HttpMethodType.Put => HttpMethod.Put,
            HttpMethodType.Patch => HttpMethod.Patch,
            HttpMethodType.Delete => HttpMethod.Delete,
            _ => HttpMethod.Post
        };
    }
}
