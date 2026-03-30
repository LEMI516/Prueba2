namespace AML.SDK.Models;

public sealed class IntentRequestDto
{
    public Guid ClientId { get; init; }
    public string Intent { get; init; } = string.Empty;
    public Dictionary<string, object?> Parameters { get; init; } = new();
}

public sealed class ServiceResponseDto
{
    public string CorrelationId { get; init; } = string.Empty;
    public bool Success { get; init; }
    public int? StatusCode { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, object?> Data { get; init; } = new();
}
