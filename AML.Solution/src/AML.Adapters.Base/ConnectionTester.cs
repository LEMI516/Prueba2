using AML.Core.Contracts.Services;
using AML.Core.Models.Requests;
using AML.Core.Models.Responses;

namespace AML.Adapters.Base;

public sealed class ConnectionTester : IConnectionTester
{
    public Task<ServiceTestResult> TestConnectionAsync(
        ServiceTestRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        var result = new ServiceTestResult
        {
            Success = true,
            StatusCode = 200,
            Message = $"Test base OK para serviceId={request.ClientServiceId}",
            DurationMs = 1
        };

        return Task.FromResult(result);
    }
}
