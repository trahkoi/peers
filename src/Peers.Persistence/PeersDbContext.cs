using Microsoft.EntityFrameworkCore;

namespace Peers.Persistence;

public sealed class PeersDbContext : DbContext
{
    private readonly IEnumerable<IModuleEntityConfiguration> _configurations;

    public PeersDbContext(
        DbContextOptions<PeersDbContext> options,
        IEnumerable<IModuleEntityConfiguration> configurations)
        : base(options)
    {
        _configurations = configurations;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var configuration in _configurations)
            configuration.Apply(modelBuilder);
    }
}
