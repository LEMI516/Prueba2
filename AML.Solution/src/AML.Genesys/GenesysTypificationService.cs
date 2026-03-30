using AML.Genesys.DTOs;

namespace AML.Genesys;

public sealed class GenesysTypificationService
{
    public string BuildTypification(GenesysEventDto eventDto)
    {
        return $"{eventDto.EventType}:{eventDto.ConversationId}";
    }
}
