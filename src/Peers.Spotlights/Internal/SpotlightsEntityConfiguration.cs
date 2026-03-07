using Microsoft.EntityFrameworkCore;
using Peers.Persistence;

namespace Peers.Spotlights.Internal;

internal sealed class SpotlightsEntityConfiguration : IModuleEntityConfiguration
{
    public void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SpotlightRoundEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SessionId).IsRequired();
            entity.Property(e => e.RoundNumber).IsRequired();
            entity.HasIndex(e => e.SessionId);
        });

        modelBuilder.Entity<PairingEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LeaderDancerId).IsRequired();
            entity.Property(e => e.FollowerDancerId).IsRequired();
            entity.Property(e => e.Order).IsRequired();
            entity.HasOne<SpotlightRoundEntity>()
                  .WithMany(r => r.Pairings)
                  .HasForeignKey(e => e.RoundId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
