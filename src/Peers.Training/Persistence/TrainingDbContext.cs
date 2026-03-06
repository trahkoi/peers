using Microsoft.EntityFrameworkCore;
using Peers.Training.Sessions;
using Peers.Training.Sessions.Internal;

namespace Peers.Training.Persistence;

public sealed class TrainingDbContext : DbContext
{
    public TrainingDbContext(DbContextOptions<TrainingDbContext> options) : base(options) { }

    internal DbSet<SessionEntity> Sessions => Set<SessionEntity>();

    internal DbSet<ParticipantEntity> Participants => Set<ParticipantEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SessionEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.HasIndex(e => e.InviteCode).IsUnique().HasFilter("InviteCode IS NOT NULL");
            entity.HasMany(e => e.Participants).WithOne(e => e.Session).HasForeignKey(e => e.SessionId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ParticipantEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DancerName).IsRequired();
            entity.HasIndex(e => new { e.SessionId, e.DancerName }).IsUnique();
            entity.HasIndex(e => e.Token).IsUnique().HasFilter("Token IS NOT NULL");
        });
    }
}
