namespace Peers.Spotlights.Internal;

internal sealed class SpotlightRoundEntity
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<PairingEntity> Pairings { get; set; } = [];
}
