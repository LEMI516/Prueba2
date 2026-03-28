using AML.Core.Enums;

namespace AML.Core.Entities;

public sealed class Client
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<ClientService> Services { get; set; } = new List<ClientService>();
}

public sealed class ClientService
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IntentKey { get; set; } = string.Empty;
    public ServiceType ServiceType { get; set; } = ServiceType.Custom;
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 1;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public Client? Client { get; set; }
    public ServiceEndpoint? Endpoint { get; set; }
    public ServiceAuth? Auth { get; set; }
    public ICollection<ServiceHeader> Headers { get; set; } = new List<ServiceHeader>();
    public ICollection<ServiceFieldMapping> FieldMappings { get; set; } = new List<ServiceFieldMapping>();
    public ICollection<ServiceLog> Logs { get; set; } = new List<ServiceLog>();
}

public sealed class ServiceEndpoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientServiceId { get; set; }
    public string Url { get; set; } = string.Empty;
    public HttpMethodType HttpMethod { get; set; } = HttpMethodType.Post;
    public int TimeoutSeconds { get; set; } = 30;

    public ClientService? ClientService { get; set; }
}

public sealed class ServiceAuth
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientServiceId { get; set; }
    public AuthType AuthType { get; set; } = AuthType.None;
    public string? ApiKeyHeaderName { get; set; }
    public string? EncryptedApiKey { get; set; }
    public string? EncryptedUsername { get; set; }
    public string? EncryptedPassword { get; set; }
    public string? EncryptedClientId { get; set; }
    public string? EncryptedClientSecret { get; set; }
    public string? TokenUrl { get; set; }
    public string? Scope { get; set; }

    public ClientService? ClientService { get; set; }
}

public sealed class ServiceHeader
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientServiceId { get; set; }
    public string HeaderKey { get; set; } = string.Empty;
    public string HeaderValue { get; set; } = string.Empty;
    public bool IsSensitive { get; set; }

    public ClientService? ClientService { get; set; }
}

public sealed class ServiceFieldMapping
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientServiceId { get; set; }
    public string SourceField { get; set; } = string.Empty;
    public string TargetField { get; set; } = string.Empty;
    public MappingDirection Direction { get; set; } = MappingDirection.Both;
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }

    public ClientService? ClientService { get; set; }
}

public sealed class ServiceLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientServiceId { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public int? StatusCode { get; set; }
    public bool Success { get; set; }
    public long DurationMs { get; set; }
    public string? RequestPayload { get; set; }
    public string? ResponsePayload { get; set; }
    public string? ErrorMessage { get; set; }

    public ClientService? ClientService { get; set; }
}
