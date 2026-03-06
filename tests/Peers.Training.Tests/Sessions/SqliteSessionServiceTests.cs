using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Peers.Persistence;
using Peers.Training.Sessions;
using Peers.Training.Sessions.Internal;

namespace Peers.Training.Tests.Sessions;

public sealed class SqliteSessionServiceTests : SessionServiceSpecificationTests
{
    private SqliteConnection? _connection;

    protected override ISessionService CreateService()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var services = new ServiceCollection();
        services.AddDbContext<PeersDbContext>(options => options.UseSqlite(_connection));
        services.AddTraining();

        var provider = services.BuildServiceProvider();

        var db = provider.GetRequiredService<PeersDbContext>();
        db.Database.EnsureCreated();

        return provider.GetRequiredService<ISessionService>();
    }

    public override void Dispose()
    {
        _connection?.Dispose();
        base.Dispose();
    }
}
