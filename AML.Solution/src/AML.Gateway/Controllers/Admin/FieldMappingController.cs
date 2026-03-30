using Microsoft.AspNetCore.Mvc;

namespace AML.Gateway.Controllers.Admin;

[ApiController]
[Route("api/admin/clients/{clientId:guid}/services/{serviceId:guid}/mappings")]
public sealed class FieldMappingController : ControllerBase
{
    [HttpGet]
    public IActionResult GetMappings(Guid clientId, Guid serviceId)
    {
        _ = clientId;
        _ = serviceId;

        return Ok(Array.Empty<object>());
    }

    [HttpPost]
    public IActionResult CreateMapping(Guid clientId, Guid serviceId, [FromBody] object payload)
    {
        _ = clientId;
        _ = serviceId;
        _ = payload;

        return Created(string.Empty, payload);
    }
}
