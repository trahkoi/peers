namespace Peers.Training.Sessions.Internal;

internal sealed class ParticipantEntity
{
    public int Id { get; set; }

    public Guid SessionId { get; set; }

    public Guid DancerId { get; set; }

    public string DancerName { get; set; } = string.Empty;

    public SessionRole Role { get; set; }

    public Guid? Token { get; set; }

    public SessionEntity Session { get; set; } = null!;
}
