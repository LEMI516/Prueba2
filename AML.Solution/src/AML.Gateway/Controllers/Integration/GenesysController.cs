using AML.Genesys;
using AML.Genesys.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AML.Gateway.Controllers.Integration;

[ApiController]
[Route("api/integration/genesys")]
public sealed class GenesysController(GenesysWebhookHandler webhookHandler) : ControllerBase
{
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] GenesysEventDto payload, CancellationToken cancellationToken)
    {
        await webhookHandler.HandleAsync(payload, cancellationToken);
        return Accepted();
    }
}
