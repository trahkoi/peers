using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Peers.Persistence;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPeersDb(this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<PeersDbContext>(options => options.UseSqlite(connectionString));

        return services;
    }

    public static void MigratePeersDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeersDbContext>();
        db.Database.Migrate();
    }
}
