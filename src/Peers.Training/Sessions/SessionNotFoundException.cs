namespace Peers.Training.Sessions;

public sealed class SessionNotFoundException : Exception
{
    public SessionNotFoundException(Guid sessionId)
        : base($"Session '{sessionId}' was not found.")
    {
        SessionId = sessionId;
    }

    public Guid SessionId { get; }
}
