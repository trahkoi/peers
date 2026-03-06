namespace Peers.Training.Sessions;

public sealed record SessionSummary(
    Guid SessionId,
    string Name,
    bool IsEnded,
    int ParticipantCount,
    string? InviteCode = null);
