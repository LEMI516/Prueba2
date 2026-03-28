namespace AML.Core.Models.Requests;

public sealed class ServiceRequest
{
    public Guid ClientId { get; set; }
    public string Intent { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString("N");
    public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();
}
