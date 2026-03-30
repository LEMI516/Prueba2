using AML.Core.Contracts.Repositories;
using AML.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AML.Infrastructure.Persistence.Repositories;

public sealed class ServiceLogRepository(AmlDbContext dbContext) : IServiceLogRepository
{
    public async Task AddAsync(ServiceLog log, CancellationToken cancellationToken = default)
    {
        await dbContext.ServiceLogs.AddAsync(log, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

}
