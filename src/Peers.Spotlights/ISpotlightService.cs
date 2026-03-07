namespace Peers.Spotlights;

public interface ISpotlightService
{
    SpotlightRound GenerateRound(Guid sessionId,
        IReadOnlyList<Guid> leaderDancerIds,
        IReadOnlyList<Guid> followerDancerIds);

    IReadOnlyList<SpotlightRound> GetRounds(Guid sessionId);

    void DeleteRound(Guid roundId);
}
