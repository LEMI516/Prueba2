using AML.Core.Models.Requests;
using AML.Core.Models.Responses;

namespace AML.Orchestrator;

public sealed class FallbackHandler
{
    public ServiceResponse BuildServiceUnavailable(ServiceRequest request, string reason)
    {
        return new ServiceResponse
        {
            CorrelationId = request.CorrelationId,
            Success = false,
            StatusCode = 503,
            Message = reason,
            Data = new Dictionary<string, object?>
            {
                ["fallback"] = true
            }
        };
    }
}
