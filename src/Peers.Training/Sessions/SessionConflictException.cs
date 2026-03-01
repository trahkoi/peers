namespace Peers.Training.Sessions;

public sealed class SessionConflictException : Exception
{
    public SessionConflictException(string message)
        : base(message)
    {
    }
}
