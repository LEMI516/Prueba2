using AML.Genesys.DTOs;

namespace AML.Genesys;

public sealed class GenesysWebhookHandler
{
    public Task HandleAsync(GenesysEventDto eventDto, CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        // Punto de extensión para eventos de llamada y estado de conversación.
        return Task.CompletedTask;
    }
}
