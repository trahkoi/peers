using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

    public static IServiceCollection AddSqliteSessions(this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<TrainingDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<ISessionService, SqliteSessionService>();

        return services;
    }

    public static void MigrateSessionsDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TrainingDbContext>();
        db.Database.Migrate();
    }
}
