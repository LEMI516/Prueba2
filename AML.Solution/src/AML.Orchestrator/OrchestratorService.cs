using AML.Core.Contracts.Adapters;
using AML.Core.Contracts.Repositories;
using AML.Core.Contracts.Services;
using AML.Core.Entities;
using AML.Core.Exceptions;
using AML.Core.Models.Requests;
using AML.Core.Models.Responses;

namespace AML.Orchestrator;

public sealed class OrchestratorService(
    IClientServiceRepository clientServiceRepository,
    IAdapterRegistry adapterRegistry,
    IBusinessRuleEngine businessRuleEngine,
    IServiceLogRepository serviceLogRepository) : IOrchestrator
{
    public async Task<ServiceResponse> ProcessAsync(ServiceRequest request, CancellationToken cancellationToken = default)
    {
        var service = await clientServiceRepository.GetByClientAndIntentAsync(
            request.ClientId,
            request.Intent,
            cancellationToken);

        if (service is null)
        {
            throw new AdapterNotFoundException(
                $"No existe un servicio activo para clientId={request.ClientId} intent={request.Intent}");
        }

        var startedAt = DateTime.UtcNow;
        try
        {
            await businessRuleEngine.ValidateAsync(service, request, cancellationToken);

            var adapter = adapterRegistry.Resolve(service);
            var response = await adapter.ExecuteAsync(service, request, cancellationToken);

            await LogAsync(service, request, response, startedAt, null, cancellationToken);

            return response;
        }
        catch (Exception ex) when (ex is not BusinessRuleViolationException)
        {
            var failed = new ServiceResponse
            {
                CorrelationId = request.CorrelationId,
                Success = false,
                Message = ex.Message
            };

            await LogAsync(service, request, failed, startedAt, ex, cancellationToken);
            throw;
        }
    }

    private async Task LogAsync(
        ClientService service,
        ServiceRequest request,
        ServiceResponse response,
        DateTime startedAt,
        Exception? exception,
        CancellationToken cancellationToken)
    {
        var duration = (long)(DateTime.UtcNow - startedAt).TotalMilliseconds;

        var log = new ServiceLog
        {
            ClientServiceId = service.Id,
            CorrelationId = request.CorrelationId,
            RequestedAtUtc = startedAt,
            StatusCode = response.StatusCode,
            Success = response.Success,
            DurationMs = duration,
            RequestPayload = System.Text.Json.JsonSerializer.Serialize(request.Parameters),
            ResponsePayload = System.Text.Json.JsonSerializer.Serialize(response.Data),
            ErrorMessage = exception?.Message
        };

        await serviceLogRepository.AddAsync(log, cancellationToken);
    }
}
