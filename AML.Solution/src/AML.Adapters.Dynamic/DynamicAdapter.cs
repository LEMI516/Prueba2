using AML.Adapters.Base;
using AML.Core.Entities;

namespace AML.Adapters.Dynamic;

public sealed class DynamicAdapter(HttpClient httpClient) : BaseAdapter(httpClient)
{
    public override string Name => "dynamic";

    public bool CanHandle(ClientService service) => service.Endpoint is not null && service.IsActive;
}
