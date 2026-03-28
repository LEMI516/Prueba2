using AML.SDK.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace AML.SDK;

public sealed class AmlGatewayClient(HttpClient httpClient, AmlGatewayClientOptions options) : IAmlGatewayClient
{
    public async Task<ServiceResponseDto> ProcessIntentAsync(
        IntentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var endpoint = options.IntegrationEndpoint.TrimStart('/');
        using var response = await httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Error invocando AML Gateway [{(int)response.StatusCode}]: {content}");
        }

        var payload = await response.Content.ReadFromJsonAsync<ServiceResponseDto>(
            new JsonSerializerOptions(JsonSerializerDefaults.Web),
            cancellationToken);

        return payload ?? new ServiceResponseDto
        {
            CorrelationId = string.Empty,
            Success = false,
            Message = "No fue posible deserializar la respuesta del gateway."
        };
    }
}
