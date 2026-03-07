namespace Peers.Spotlights;

public interface ISpotlightService
{
    SpotlightRound GenerateRound(Guid sessionId,
        IReadOnlyList<Guid> leaderDancerIds,
        IReadOnlyList<Guid> followerDancerIds);

    SpotlightRound? GetRound(Guid sessionId);
}
