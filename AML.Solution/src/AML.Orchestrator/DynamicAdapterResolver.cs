using AML.Core.Entities;
using AML.Core.Contracts.Adapters;

namespace AML.Orchestrator;

public sealed class DynamicAdapterResolver
{
    public bool ShouldUseDynamicAdapter(ClientService service)
    {
        return service.Endpoint is not null && service.IsActive;
    }

    public IServiceAdapter Resolve(ClientService service, IEnumerable<IServiceAdapter> adapters)
    {
        if (ShouldUseDynamicAdapter(service))
        {
            var dynamicAdapter = adapters.FirstOrDefault(
                x => x.Name.Equals("dynamic", StringComparison.OrdinalIgnoreCase));

            if (dynamicAdapter is not null)
            {
                return dynamicAdapter;
            }
        }

        return adapters.First();
    }
}
