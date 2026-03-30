using Microsoft.AspNetCore.Mvc;

namespace AML.Gateway.Controllers.Admin;

[ApiController]
[Route("api/admin/service-headers")]
public sealed class ServiceHeaderController : ControllerBase
{
    [HttpGet("{clientServiceId:guid}")]
    public IActionResult GetByService(Guid clientServiceId)
    {
        return Ok(new
        {
            clientServiceId,
            items = Array.Empty<object>()
        });
    }
}
