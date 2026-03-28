namespace AML.Genesys.DTOs;

public sealed class GenesysEventDto
{
    public string EventType { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
    public string? ParticipantId { get; set; }
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object?> Payload { get; set; } = new();
}

public sealed class GenesysTransferDto
{
    public string ConversationId { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
