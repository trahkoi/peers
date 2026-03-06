using Microsoft.EntityFrameworkCore;
using Peers.Persistence;
using Peers.Training.Dancers;
using Peers.Training.Sessions.Internal;

namespace Peers.Training.Persistence;

internal sealed class TrainingEntityConfiguration : IModuleEntityConfiguration
{
    public void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DancerEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });

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
            entity.HasOne<DancerEntity>().WithMany().HasForeignKey(e => e.DancerId);
        });
    }
}
