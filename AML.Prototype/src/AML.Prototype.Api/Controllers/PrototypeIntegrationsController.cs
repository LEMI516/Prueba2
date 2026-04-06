using AML.Prototype.Contracts.Models;
using AML.Prototype.Engine.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AML.Prototype.Api.Controllers;

[ApiController]
[Route("api/prototype/integrations")]
public sealed class PrototypeIntegrationsController(IIntegrationDefinitionStore definitionStore) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyCollection<IntegrationDefinition>> GetAll()
    {
        return Ok(definitionStore.GetAll());
    }

    [HttpGet("{key}")]
    public ActionResult<IntegrationDefinition> GetByKey(string key)
    {
        var item = definitionStore.GetByKey(key);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost("{key}")]
    public ActionResult<IntegrationDefinition> Create(string key, [FromBody] UpsertIntegrationDefinitionRequest request)
    {
        if (definitionStore.GetByKey(key) is not null)
        {
            return Conflict(new { message = $"Ya existe una integración con key '{key}'." });
        }

        var created = definitionStore.Upsert(key, request);
        return CreatedAtAction(nameof(GetByKey), new { key = created.Key }, created);
    }

    [HttpPut("{key}")]
    public ActionResult<IntegrationDefinition> Upsert(string key, [FromBody] UpsertIntegrationDefinitionRequest request)
    {
        var updated = definitionStore.Upsert(key, request);
        return Ok(updated);
    }

    [HttpDelete("{key}")]
    public IActionResult Delete(string key)
    {
        var deleted = definitionStore.Delete(key);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("seed-local")]
    public IActionResult SeedLocal()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        definitionStore.Upsert("mock-billing", new UpsertIntegrationDefinitionRequest
        {
            Name = "Mock Billing",
            BaseUrl = baseUrl,
            Path = "/api/prototype/mock/billing/{customerId}",
            Method = PrototypeHttpMethod.Get,
            TimeoutSeconds = 20
        });

        definitionStore.Upsert("mock-payment", new UpsertIntegrationDefinitionRequest
        {
            Name = "Mock Payment",
            BaseUrl = baseUrl,
            Path = "/api/prototype/mock/payment/authorize",
            Method = PrototypeHttpMethod.Post,
            TimeoutSeconds = 20
        });

        return Ok(new
        {
            message = "Integraciones demo creadas.",
            baseUrl
        });
    }
}
