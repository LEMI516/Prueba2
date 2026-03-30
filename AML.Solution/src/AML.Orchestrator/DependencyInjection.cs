using AML.Core.Contracts.Adapters;
using AML.Core.Contracts.Services;
using AML.Orchestrator;
using Microsoft.Extensions.DependencyInjection;

namespace AML.Orchestrator;

public static class DependencyInjection
{
    public static IServiceCollection AddAmlOrchestrator(this IServiceCollection services)
    {
        services.AddScoped<IOrchestrator, OrchestratorService>();
        services.AddScoped<IAdapterRegistry, AdapterRegistry>();
        services.AddScoped<DynamicAdapterResolver>();
        services.AddScoped<FallbackHandler>();

        return services;
    }
}
