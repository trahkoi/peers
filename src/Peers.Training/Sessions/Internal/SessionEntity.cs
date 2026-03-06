namespace Peers.Training.Sessions.Internal;

internal sealed class SessionEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsEnded { get; set; }

    public string? InviteCode { get; set; }

    public List<ParticipantEntity> Participants { get; set; } = [];
}
