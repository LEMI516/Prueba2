using AML.Core.Contracts.Repositories;
using AML.Infrastructure.Persistence;
using AML.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AML.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAmlInfrastructure(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<AmlDbContext>(configureDb);

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IClientServiceRepository, ClientServiceRepository>();
        services.AddScoped<IServiceLogRepository, ServiceLogRepository>();

        return services;
    }
}
