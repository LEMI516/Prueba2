using AML.Prototype.Contracts.Models;

namespace AML.Prototype.Engine.Abstractions;

public interface IIntegrationDefinitionStore
{
    IReadOnlyCollection<IntegrationDefinition> GetAll();
    IntegrationDefinition? GetByKey(string key);
    IntegrationDefinition Upsert(string key, UpsertIntegrationDefinitionRequest request);
    bool Delete(string key);
}
