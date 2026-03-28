using AML.Core.Entities;
using AML.Core.Models.Requests;
using AML.Core.Models.Responses;

namespace AML.Core.Contracts.Services;

public interface IOrchestrator
{
    Task<ServiceResponse> ProcessAsync(ServiceRequest request, CancellationToken cancellationToken = default);
}

public interface IBusinessRuleEngine
{
    Task ValidateAsync(ClientService service, ServiceRequest request, CancellationToken cancellationToken = default);
}

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default);
}

public interface IConnectionTester
{
    Task<ServiceTestResult> TestConnectionAsync(ServiceTestRequest request, CancellationToken cancellationToken = default);
}
