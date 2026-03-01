using Microsoft.Extensions.DependencyInjection;
using Peers.Training.Sessions.Internal;

namespace Peers.Training.Sessions;

public static class SessionServiceCollectionExtensions
{
    public static IServiceCollection AddSessions(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ISessionService>(static _ => new InMemorySessionService());

        return services;
    }
}
