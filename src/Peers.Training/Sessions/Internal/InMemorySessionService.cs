namespace Peers.Training.Sessions.Internal;

internal sealed class InMemorySessionService : ISessionService
{
    private readonly object _sync = new();
    private readonly Dictionary<Guid, SessionState> _sessions = [];

    public Guid CreateSession(string name)
    {
        var normalizedName = RequireName(name, "Session name");
        var sessionId = Guid.NewGuid();

        lock (_sync)
        {
            _sessions[sessionId] = new SessionState(sessionId, normalizedName);
            return sessionId;
        }
    }

    public void EndSession(Guid sessionId)
    {
        lock (_sync)
        {
            var session = GetSession(sessionId);
            if (session.Status == SessionStatus.Ended)
            {
                throw new SessionConflictException($"Session '{sessionId}' is already ended.");
            }

            session.Status = SessionStatus.Ended;
        }
    }

    public void JoinSession(Guid sessionId, string dancerName, SessionRole role)
    {
        var normalizedDancerName = RequireName(dancerName, "Dancer name");
        var normalizedRole = NormalizeRole(role);

        lock (_sync)
        {
            var session = GetSession(sessionId);
            EnsureSessionIsActive(session);

            if (session.Participants.ContainsKey(normalizedDancerName))
            {
                throw new SessionConflictException(
                    $"Dancer '{normalizedDancerName}' is already participating in session '{sessionId}'.");
            }

            session.Participants[normalizedDancerName] = new ParticipantState(normalizedDancerName, normalizedRole);
        }
    }

    public void LeaveSession(Guid sessionId, string dancerName)
    {
        var normalizedDancerName = RequireName(dancerName, "Dancer name");

        lock (_sync)
        {
            var session = GetSession(sessionId);
            EnsureSessionIsActive(session);

            if (!session.Participants.Remove(normalizedDancerName))
            {
                throw new SessionNotFoundException(sessionId);
            }
        }
    }

    public IReadOnlyList<Participant> ListParticipants(Guid sessionId)
    {
        lock (_sync)
        {
            var session = GetSession(sessionId);
            return session.Participants.Values
                .OrderBy(x => x.DancerName, StringComparer.OrdinalIgnoreCase)
                .Select(x => new Participant(x.DancerName, x.Role))
                .ToArray();
        }
    }

    private SessionState GetSession(Guid sessionId)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            throw new SessionNotFoundException(sessionId);
        }

        return session;
    }

    private static string RequireName(string name, string paramName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new SessionValidationException($"{paramName} is required.");
        }

        return name.Trim();
    }

    private static SessionRole NormalizeRole(SessionRole role)
    {
        if (!Enum.IsDefined(role))
        {
            throw new SessionValidationException(
                $"Role '{role}' is invalid. Supported values are '{SessionRole.Leader}' and '{SessionRole.Follower}'.");
        }

        return role;
    }

    private static void EnsureSessionIsActive(SessionState session)
    {
        if (session.Status == SessionStatus.Ended)
        {
            throw new SessionConflictException($"Session '{session.Id}' has ended and can no longer be modified.");
        }
    }

    private sealed class SessionState
    {
        public SessionState(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }

        public SessionStatus Status { get; set; } = SessionStatus.Active;

        public Dictionary<string, ParticipantState> Participants { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private sealed record ParticipantState(string DancerName, SessionRole Role);

    private enum SessionStatus
    {
        Active,
        Ended
    }
}
