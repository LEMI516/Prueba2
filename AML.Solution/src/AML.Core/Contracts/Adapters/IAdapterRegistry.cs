using AML.Core.Entities;
using AML.Core.Models.Requests;
using AML.Core.Models.Responses;

namespace AML.Core.Contracts.Adapters;

public interface IAdapterRegistry
{
    IServiceAdapter Resolve(ClientService clientService);
    void Register(IServiceAdapter adapter);
}
