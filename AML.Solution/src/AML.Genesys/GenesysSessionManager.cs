namespace AML.Genesys;

public sealed class GenesysSessionManager
{
    private readonly Dictionary<string, DateTime> _sessions = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lock = new();

    public void Touch(string conversationId)
    {
        lock (_lock)
        {
            _sessions[conversationId] = DateTime.UtcNow;
        }
    }

    public bool IsActive(string conversationId, TimeSpan maxInactivity)
    {
        lock (_lock)
        {
            return _sessions.TryGetValue(conversationId, out var lastSeen)
                   && DateTime.UtcNow - lastSeen <= maxInactivity;
        }
    }
}
