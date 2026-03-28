using AML.Core.Contracts.Repositories;
using AML.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AML.Gateway.Controllers.Admin;

[ApiController]
[Route("api/admin/clients")]
public sealed class ClientController(IClientRepository clientRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<Client>>> GetAll(CancellationToken cancellationToken)
    {
        var clients = await clientRepository.GetAllAsync(cancellationToken);
        return Ok(clients);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Client>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(id, cancellationToken);
        return client is null ? NotFound() : Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<Client>> Create([FromBody] Client request, CancellationToken cancellationToken)
    {
        await clientRepository.AddAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
    }
}
