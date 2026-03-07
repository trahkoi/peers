using Peers.Persistence;

namespace Peers.Persistence.Tests;

public sealed class EnsureDatabaseDirectoryExistsTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    [Fact]
    public void Creates_nested_directory_when_it_does_not_exist()
    {
        var nestedDir = Path.Combine(_tempDir, "nested");
        var connectionString = $"Data Source={Path.Combine(nestedDir, "peers.db")}";

        PersistenceServiceCollectionExtensions.EnsureDatabaseDirectoryExists(connectionString);

        Assert.True(Directory.Exists(nestedDir));
    }

    [Fact]
    public void Does_not_throw_for_in_memory_database()
    {
        PersistenceServiceCollectionExtensions.EnsureDatabaseDirectoryExists("Data Source=:memory:");
    }

    [Fact]
    public void Does_not_throw_when_directory_already_exists()
    {
        Directory.CreateDirectory(_tempDir);
        var connectionString = $"Data Source={Path.Combine(_tempDir, "peers.db")}";

        PersistenceServiceCollectionExtensions.EnsureDatabaseDirectoryExists(connectionString);

        Assert.True(Directory.Exists(_tempDir));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }
}
