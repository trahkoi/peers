namespace Peers.Spotlights.Internal;

internal sealed class PairingEntity
{
    public Guid Id { get; set; }

    public Guid RoundId { get; set; }

    public Guid LeaderDancerId { get; set; }

    public Guid FollowerDancerId { get; set; }

    public int Order { get; set; }
}
