namespace Peers.Spotlights;

public sealed record SpotlightRound(Guid Id, Guid SessionId, int RoundNumber, DateTime CreatedAt, IReadOnlyList<Pairing> Pairings);
