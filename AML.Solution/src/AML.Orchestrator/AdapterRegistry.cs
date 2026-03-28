using AML.Core.Contracts.Adapters;
using AML.Core.Entities;

namespace AML.Orchestrator;

public sealed class AdapterRegistry(
    DynamicAdapterResolver dynamicAdapterResolver,
    IEnumerable<IServiceAdapter> adapters) : IAdapterRegistry
{
    private readonly List<IServiceAdapter> _runtimeAdapters = new();

    public IServiceAdapter Resolve(ClientService clientService)
    {
        if (_runtimeAdapters.Count > 0)
        {
            var selected = dynamicAdapterResolver.Resolve(clientService, _runtimeAdapters);
            if (selected is not null)
            {
                return selected;
            }
        }

        var candidate = dynamicAdapterResolver.Resolve(clientService, adapters);
        if (candidate is not null)
        {
            return candidate;
        }

        return adapters.First();
    }

    public void Register(IServiceAdapter adapter)
    {
        _runtimeAdapters.Add(adapter);
    }
}
