namespace Peers.Training.Sessions;

public sealed record Participant(Guid DancerId, string DancerName, SessionRole Role);
