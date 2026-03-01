using Peers.Training.Sessions.Internal;

namespace Peers.Training.Sessions;

public static class SessionServiceFactory
{
    public static ISessionService CreateInMemory() => new InMemorySessionService();
}
