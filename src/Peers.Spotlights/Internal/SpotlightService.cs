using Microsoft.EntityFrameworkCore;
using Peers.Persistence;

namespace Peers.Spotlights.Internal;

internal sealed class SpotlightService : ISpotlightService
{
    private readonly PeersDbContext _db;

    private DbSet<SpotlightRoundEntity> Rounds => _db.Set<SpotlightRoundEntity>();
    private DbSet<PairingEntity> Pairings => _db.Set<PairingEntity>();

    public SpotlightService(PeersDbContext db)
    {
        _db = db;
    }

    public SpotlightRound GenerateRound(Guid sessionId,
        IReadOnlyList<Guid> leaderDancerIds,
        IReadOnlyList<Guid> followerDancerIds)
    {
        var allDancerIds = leaderDancerIds.Concat(followerDancerIds).Distinct().ToList();
        var historicalCounts = GetHistoricalCounts(allDancerIds);
        var pairings = PairingAlgorithm.GeneratePairings(leaderDancerIds, followerDancerIds, historicalCounts);

        // Remove existing round for this session (one round per session)
        var existingRound = Rounds
            .Include(r => r.Pairings)
            .FirstOrDefault(r => r.SessionId == sessionId);

        if (existingRound is not null)
        {
            Rounds.Remove(existingRound);
        }

        var round = new SpotlightRoundEntity
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            CreatedAt = DateTime.UtcNow,
            Pairings = pairings.Select((p, i) => new PairingEntity
            {
                Id = Guid.NewGuid(),
                LeaderDancerId = p.LeaderId,
                FollowerDancerId = p.FollowerId,
                Order = i + 1
            }).ToList()
        };

        Rounds.Add(round);
        _db.SaveChanges();

        return ToDto(round);
    }

    public SpotlightRound? GetRound(Guid sessionId)
    {
        var round = Rounds
            .Include(r => r.Pairings)
            .FirstOrDefault(r => r.SessionId == sessionId);

        return round is null ? null : ToDto(round);
    }

    private Dictionary<(Guid LeaderId, Guid FollowerId), int> GetHistoricalCounts(
        IReadOnlyList<Guid> dancerIds)
    {
        if (dancerIds.Count == 0)
            return [];

        var dancerIdSet = dancerIds.ToHashSet();

        return Pairings
            .Where(p => dancerIdSet.Contains(p.LeaderDancerId) || dancerIdSet.Contains(p.FollowerDancerId))
            .GroupBy(p => new { p.LeaderDancerId, p.FollowerDancerId })
            .ToDictionary(
                g => (g.Key.LeaderDancerId, g.Key.FollowerDancerId),
                g => g.Count());
    }

    private static SpotlightRound ToDto(SpotlightRoundEntity entity)
    {
        return new SpotlightRound(
            entity.Id,
            entity.SessionId,
            entity.CreatedAt,
            entity.Pairings
                .OrderBy(p => p.Order)
                .Select(p => new Pairing(p.LeaderDancerId, p.FollowerDancerId, p.Order))
                .ToList());
    }
}
