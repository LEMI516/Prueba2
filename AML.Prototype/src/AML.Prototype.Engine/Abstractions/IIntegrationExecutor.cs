using AML.Prototype.Contracts.Models;

namespace AML.Prototype.Engine.Abstractions;

public interface IIntegrationExecutor
{
    Task<IntegrationRunResult> RunAsync(RunIntegrationRequest request, CancellationToken cancellationToken = default);
}
