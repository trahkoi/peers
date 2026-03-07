namespace Peers.Spotlights;

public sealed record SpotlightRound(Guid Id, Guid SessionId, DateTime CreatedAt, IReadOnlyList<Pairing> Pairings);
