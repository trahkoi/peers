using Microsoft.Data.Sqlite;
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

    public static void MigratePeersDatabase(this IServiceProvider services, string connectionString)
    {
        EnsureDatabaseDirectoryExists(connectionString);

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeersDbContext>();
        db.Database.Migrate();
    }

    internal static void EnsureDatabaseDirectoryExists(string connectionString)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);
        var dataSource = builder.DataSource;

        if (string.IsNullOrEmpty(dataSource) || dataSource == ":memory:")
            return;

        var directory = Path.GetDirectoryName(Path.GetFullPath(dataSource));
        if (directory is not null)
            Directory.CreateDirectory(directory);
    }
}
