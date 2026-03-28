using AML.Core.Entities;
using AML.Core.Models.Requests;
using AML.Core.Models.Responses;

namespace AML.Core.Contracts.Adapters;

public interface IServiceAdapter
{
    string Name { get; }
    Task<ServiceResponse> ExecuteAsync(ClientService service, ServiceRequest request, CancellationToken cancellationToken = default);
}
