using AML.Core.Contracts.Repositories;
using AML.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AML.Infrastructure.Persistence.Repositories;

public sealed class ClientRepository : IClientRepository
{
    private readonly AmlDbContext _dbContext;

    public ClientRepository(AmlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Client>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Clients
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Clients
            .Include(x => x.Services)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Client?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Clients
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task AddAsync(Client client, CancellationToken cancellationToken = default)
    {
        await _dbContext.Clients.AddAsync(client, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Client client, CancellationToken cancellationToken = default)
    {
        _dbContext.Clients.Update(client);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
