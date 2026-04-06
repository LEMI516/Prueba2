using AML.Prototype.Contracts.Models;
using AML.Prototype.Engine.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AML.Prototype.Api.Controllers;

[ApiController]
[Route("api/prototype/executions")]
public sealed class PrototypeExecutionsController(IIntegrationExecutor integrationExecutor) : ControllerBase
{
    [HttpPost("run")]
    [ProducesResponseType(typeof(IntegrationRunResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<IntegrationRunResult>> Run(
        [FromBody] RunIntegrationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await integrationExecutor.RunAsync(request, cancellationToken);
        return Ok(result);
    }
}
