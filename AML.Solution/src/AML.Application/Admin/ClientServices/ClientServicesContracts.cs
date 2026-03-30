using AML.Core.Enums;

namespace AML.Application.Admin.ClientServices;

public sealed record CreateClientServiceCommand(
    Guid ClientId,
    string Name,
    string IntentKey,
    ServiceType ServiceType,
    string EndpointUrl,
    HttpMethodType HttpMethod,
    int TimeoutSeconds);

public sealed record UpdateClientServiceCommand(
    Guid Id,
    string Name,
    string IntentKey,
    ServiceType ServiceType,
    bool IsActive,
    int Priority);

public sealed record ToggleServiceCommand(Guid Id, bool IsActive);
public sealed record TestServiceConnectionCommand(Guid ClientServiceId, Dictionary<string, object?> Payload);

public sealed record GetServicesByClientQuery(Guid ClientId);
public sealed record GetServiceDetailQuery(Guid ServiceId);

public sealed record ClientServiceDto(
    Guid Id,
    Guid ClientId,
    string Name,
    string IntentKey,
    ServiceType ServiceType,
    bool IsActive,
    int Priority);

public sealed record ServiceEndpointDto(
    Guid Id,
    string Url,
    HttpMethodType HttpMethod,
    int TimeoutSeconds);

public sealed record ServiceAuthDto(
    Guid Id,
    AuthType AuthType,
    string? ApiKeyHeaderName,
    bool HasApiKey,
    bool HasBasicCredentials,
    bool HasOAuthCredentials,
    string? TokenUrl,
    string? Scope);

public sealed record ServiceHeaderDto(
    Guid Id,
    string HeaderKey,
    string HeaderValue,
    bool IsSensitive);

public sealed record ServiceFieldMappingDto(
    Guid Id,
    string SourceField,
    string TargetField,
    MappingDirection Direction,
    bool IsRequired,
    string? DefaultValue);

public sealed record ClientServiceDetailDto(
    Guid Id,
    Guid ClientId,
    string Name,
    string IntentKey,
    ServiceType ServiceType,
    bool IsActive,
    int Priority,
    ServiceEndpointDto? Endpoint,
    ServiceAuthDto? Auth,
    IReadOnlyCollection<ServiceHeaderDto> Headers,
    IReadOnlyCollection<ServiceFieldMappingDto> FieldMappings);
