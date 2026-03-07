namespace Peers.Training.Sessions;

public interface ISessionService
{
    IReadOnlyList<SessionSummary> ListSessions();

    Guid CreateSession(string name);

    void EndSession(Guid sessionId);

    void JoinSession(Guid sessionId, string dancerName, SessionRole role);

    void LeaveSession(Guid sessionId, string dancerName);

    IReadOnlyList<Participant> ListParticipants(Guid sessionId);

    string GenerateInviteCode(Guid sessionId);

    Guid JoinViaCode(string inviteCode, string dancerName, SessionRole role);

    (Guid SessionId, string DancerName, bool IsCoach)? GetParticipantSession(Guid token);

    void PromoteParticipant(Guid sessionId, string dancerName);

    void DemoteParticipant(Guid sessionId, string dancerName);
}
