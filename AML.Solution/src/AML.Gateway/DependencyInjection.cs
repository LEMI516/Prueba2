using AML.Adapters.Base;
using AML.Adapters.Dynamic;
using AML.Core.Contracts.Adapters;
using AML.Core.Contracts.Services;
using AML.Orchestrator;
using AML.RulesEngine;
using Microsoft.Extensions.DependencyInjection;

namespace AML.Gateway;

public static class DependencyInjection
{
    public static IServiceCollection AddGatewayCore(this IServiceCollection services)
    {
        services.AddHttpClient<DynamicAdapter>();
        services.AddScoped<IServiceAdapter, DynamicAdapter>();
        services.AddScoped<IAdapterRegistry, AdapterRegistry>();
        services.AddScoped<DynamicAdapterResolver>();
        services.AddScoped<IOrchestrator, OrchestratorService>();
        services.AddScoped<IBusinessRuleEngine, BusinessRuleEngine>();
        services.AddScoped<IConnectionTester, ConnectionTester>();
        services.AddSingleton<FallbackHandler>();

        return services;
    }
}
