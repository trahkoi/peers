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

        var nextRoundNumber = (Rounds
            .Where(r => r.SessionId == sessionId)
            .Max(r => (int?)r.RoundNumber) ?? 0) + 1;

        var round = new SpotlightRoundEntity
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            RoundNumber = nextRoundNumber,
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

    public IReadOnlyList<SpotlightRound> GetRounds(Guid sessionId)
    {
        return Rounds
            .Include(r => r.Pairings)
            .Where(r => r.SessionId == sessionId)
            .OrderBy(r => r.RoundNumber)
            .AsEnumerable()
            .Select(ToDto)
            .ToList();
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
            entity.RoundNumber,
            entity.CreatedAt,
            entity.Pairings
                .OrderBy(p => p.Order)
                .Select(p => new Pairing(p.LeaderDancerId, p.FollowerDancerId, p.Order))
                .ToList());
    }
}
