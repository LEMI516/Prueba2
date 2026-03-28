using AML.Core.Entities;

namespace AML.Core.Contracts.Repositories;

public interface IClientRepository
{
    Task<IReadOnlyCollection<Client>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Client?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(Client client, CancellationToken cancellationToken = default);
    Task UpdateAsync(Client client, CancellationToken cancellationToken = default);
}

public interface IClientServiceRepository
{
    Task<IReadOnlyCollection<ClientService>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<ClientService?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClientService?> GetByClientAndIntentAsync(
        Guid clientId,
        string intentKey,
        CancellationToken cancellationToken = default);
    Task AddAsync(ClientService service, CancellationToken cancellationToken = default);
    Task UpdateAsync(ClientService service, CancellationToken cancellationToken = default);
}

public interface IServiceLogRepository
{
    Task AddAsync(ServiceLog log, CancellationToken cancellationToken = default);
}
