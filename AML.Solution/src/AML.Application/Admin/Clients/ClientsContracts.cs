namespace AML.Application.Admin.Clients;

public sealed record CreateClientCommand(
    string Code,
    string Name,
    bool IsActive = true);

public sealed record UpdateClientCommand(
    Guid Id,
    string Name,
    bool IsActive);

public sealed record GetAllClientsQuery;

public sealed record GetClientByIdQuery(Guid Id);

public sealed record ClientDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive);

public sealed record ClientDetailDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
