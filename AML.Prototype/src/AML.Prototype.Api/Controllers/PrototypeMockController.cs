using Microsoft.AspNetCore.Mvc;

namespace AML.Prototype.Api.Controllers;

[ApiController]
[Route("api/prototype/mock")]
public sealed class PrototypeMockController : ControllerBase
{
    [HttpGet("billing/{customerId}")]
    public IActionResult GetBilling(string customerId)
    {
        var amount = (customerId.Length * 137) % 2500 + 100;
        return Ok(new
        {
            customerId,
            invoiceId = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId.ToUpperInvariant()}",
            amount,
            currency = "USD",
            dueDate = DateTime.UtcNow.Date.AddDays(10)
        });
    }

    [HttpPost("payment/authorize")]
    public IActionResult Authorize([FromBody] PaymentAuthorizationRequest request)
    {
        var approved = request.Amount <= 5000m;
        return Ok(new
        {
            request.CustomerId,
            request.Amount,
            request.Currency,
            approved,
            authorizationCode = approved ? $"AUTH-{Guid.NewGuid():N}" : null,
            message = approved ? "Autorización exitosa." : "Monto supera límite del mock."
        });
    }

    public sealed class PaymentAuthorizationRequest
    {
        public string CustomerId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
    }
}
