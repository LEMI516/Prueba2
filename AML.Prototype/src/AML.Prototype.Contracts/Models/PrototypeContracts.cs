namespace AML.Prototype.Contracts.Models;

public enum PrototypeHttpMethod
{
    Get = 1,
    Post = 2,
    Put = 3,
    Patch = 4,
    Delete = 5
}

public sealed class IntegrationDefinition
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Path { get; set; } = "/";
    public PrototypeHttpMethod Method { get; set; } = PrototypeHttpMethod.Post;
    public int TimeoutSeconds { get; set; } = 30;
    public Dictionary<string, string> DefaultHeaders { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}

public sealed class UpsertIntegrationDefinitionRequest
{
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Path { get; set; } = "/";
    public PrototypeHttpMethod Method { get; set; } = PrototypeHttpMethod.Post;
    public int TimeoutSeconds { get; set; } = 30;
    public Dictionary<string, string>? DefaultHeaders { get; set; }
}

public sealed class RunIntegrationRequest
{
    public string IntegrationKey { get; set; } = string.Empty;
    public Dictionary<string, string> PathParameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> QueryParameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Headers { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, object?> Payload { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class IntegrationRunResult
{
    public required string IntegrationKey { get; init; }
    public bool Success { get; init; }
    public int? StatusCode { get; init; }
    public required string RequestUrl { get; init; }
    public long DurationMs { get; init; }
    public string? ResponseBody { get; init; }
    public string? ErrorMessage { get; init; }
}
