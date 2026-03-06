using Microsoft.Extensions.DependencyInjection;
using Peers.Persistence;
using Peers.Training.Persistence;
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

    public static IServiceCollection AddTraining(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IModuleEntityConfiguration, TrainingEntityConfiguration>();
        services.AddScoped<ISessionService, SqliteSessionService>();

        return services;
    }
}
