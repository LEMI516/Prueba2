using AML.Application.Integration.DTOs;
using AML.Core.Contracts.Services;
using AML.Core.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AML.Gateway.Controllers.Integration;

[ApiController]
[Route("api/integration")]
public sealed class IntegrationController(IOrchestrator orchestrator) : ControllerBase
{
    [HttpPost("process-intent")]
    [ProducesResponseType(typeof(IntentResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<IntentResponseDto>> ProcessIntent(
        [FromBody] IntentRequestDto request,
        CancellationToken cancellationToken)
    {
        var domainRequest = new ServiceRequest
        {
            ClientId = request.ClientId,
            Intent = request.Intent,
            Parameters = request.Parameters
        };

        var domainResponse = await orchestrator.ProcessAsync(domainRequest, cancellationToken);

        var response = new IntentResponseDto
        {
            CorrelationId = domainResponse.CorrelationId,
            Success = domainResponse.Success,
            StatusCode = domainResponse.StatusCode,
            Message = domainResponse.Message,
            Data = domainResponse.Data
        };

        return Ok(response);
    }
}
