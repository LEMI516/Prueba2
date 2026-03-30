using AML.Core.Contracts.Repositories;
using AML.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AML.Infrastructure.Persistence.Repositories;

public sealed class ClientServiceRepository(AmlDbContext dbContext) : IClientServiceRepository
{
    public async Task<IReadOnlyCollection<ClientService>> GetByClientIdAsync(
        Guid clientId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ClientServices
            .AsNoTracking()
            .Include(x => x.Endpoint)
            .Include(x => x.Auth)
            .Include(x => x.Headers)
            .Include(x => x.FieldMappings)
            .Where(x => x.ClientId == clientId)
            .OrderBy(x => x.Priority)
            .ToListAsync(cancellationToken);
    }

    public Task<ClientService?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.ClientServices
            .Include(x => x.Endpoint)
            .Include(x => x.Auth)
            .Include(x => x.Headers)
            .Include(x => x.FieldMappings)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<ClientService?> GetByClientAndIntentAsync(
        Guid clientId,
        string intentKey,
        CancellationToken cancellationToken = default)
    {
        return dbContext.ClientServices
            .Include(x => x.Endpoint)
            .Include(x => x.Auth)
            .Include(x => x.Headers)
            .Include(x => x.FieldMappings)
            .FirstOrDefaultAsync(
                x => x.ClientId == clientId && x.IntentKey == intentKey && x.IsActive,
                cancellationToken);
    }

    public async Task AddAsync(ClientService service, CancellationToken cancellationToken = default)
    {
        await dbContext.ClientServices.AddAsync(service, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ClientService service, CancellationToken cancellationToken = default)
    {
        dbContext.ClientServices.Update(service);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
