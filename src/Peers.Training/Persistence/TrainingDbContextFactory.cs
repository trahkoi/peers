using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Peers.Training.Persistence;

public sealed class TrainingDbContextFactory : IDesignTimeDbContextFactory<TrainingDbContext>
{
    public TrainingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TrainingDbContext>()
            .UseSqlite("Data Source=peers.db")
            .Options;

        return new TrainingDbContext(options);
    }
}
