using Microsoft.EntityFrameworkCore;

namespace Peers.Persistence;

public interface IModuleEntityConfiguration
{
    void Apply(ModelBuilder modelBuilder);
}
