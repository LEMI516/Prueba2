namespace AML.Core.Models.Responses;

public sealed class ServiceResponse
{
    public required string CorrelationId { get; init; }
    public bool Success { get; init; }
    public int? StatusCode { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, object?> Data { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}
