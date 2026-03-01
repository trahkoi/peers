namespace Peers.Training.Sessions;

public interface ISessionService
{
    Guid CreateSession(string name);

    void EndSession(Guid sessionId);

    void JoinSession(Guid sessionId, string dancerName, SessionRole role);

    void LeaveSession(Guid sessionId, string dancerName);

    IReadOnlyList<Participant> ListParticipants(Guid sessionId);
}
