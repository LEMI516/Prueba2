using AML.Core.Contracts.Services;
using AML.Core.Entities;
using AML.Core.Enums;
using AML.Core.Exceptions;
using AML.Core.Models.Requests;

namespace AML.RulesEngine;

public sealed class BusinessRuleEngine : IBusinessRuleEngine
{
    public Task ValidateAsync(
        ClientService service,
        ServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!service.IsActive)
        {
            throw new BusinessRuleViolationException("El servicio se encuentra inactivo.");
        }

        if (string.IsNullOrWhiteSpace(request.Intent))
        {
            throw new BusinessRuleViolationException("La intención no puede estar vacía.");
        }

        if (service.ServiceType == ServiceType.Payment &&
            !request.Parameters.ContainsKey("amount"))
        {
            throw new BusinessRuleViolationException("Regla de pago: falta el campo amount.");
        }

        return Task.CompletedTask;
    }
}
