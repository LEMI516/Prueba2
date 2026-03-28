namespace AML.Core.Models.Requests;

public sealed class ServiceTestRequest
{
    public Guid ClientServiceId { get; set; }
    public Dictionary<string, object?> SamplePayload { get; set; } = new();
}
