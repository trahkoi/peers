namespace Peers.Spotlights;

public sealed record Pairing(Guid LeaderDancerId, Guid FollowerDancerId, int Order);
