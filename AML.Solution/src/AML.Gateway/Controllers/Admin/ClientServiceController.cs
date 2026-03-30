using AML.Core.Contracts.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AML.Gateway.Controllers.Admin;

[ApiController]
[Route("api/admin/client-services")]
public sealed class ClientServiceController(IClientServiceRepository clientServiceRepository) : ControllerBase
{
    [HttpGet("client/{clientId:guid}")]
    public async Task<IActionResult> GetByClient(Guid clientId, CancellationToken cancellationToken)
    {
        var services = await clientServiceRepository.GetByClientIdAsync(clientId, cancellationToken);
        return Ok(services);
    }
}
