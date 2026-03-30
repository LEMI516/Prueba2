using AML.SDK.Models;

namespace AML.SDK;

public interface IAmlGatewayClient
{
    Task<ServiceResponseDto> ProcessIntentAsync(IntentRequestDto request, CancellationToken cancellationToken = default);
}
