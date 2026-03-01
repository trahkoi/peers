namespace Peers.Training.Sessions;

public sealed class SessionValidationException : Exception
{
    public SessionValidationException(string message)
        : base(message)
    {
    }
}
