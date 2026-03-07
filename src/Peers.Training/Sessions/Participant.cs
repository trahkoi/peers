namespace Peers.Training.Sessions;

public sealed record Participant(Guid DancerId, string DancerName, SessionRole Role, bool IsCoach = false, bool HasToken = false);
