namespace Peers.Training.Sessions.Internal;

internal sealed class InMemorySessionService : ISessionService
{
    private static readonly char[] InviteCodeAlphabet =
        "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray(); // excludes 0,O,1,I

    private readonly object _sync = new();
    private readonly Dictionary<Guid, SessionState> _sessions = [];
    private readonly Dictionary<Guid, (Guid SessionId, string DancerName)> _tokenIndex = [];

    public IReadOnlyList<SessionSummary> ListSessions()
    {
        lock (_sync)
        {
            return _sessions.Values
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SessionSummary(
                    x.Id,
                    x.Name,
                    x.Status == SessionStatus.Ended,
                    x.Participants.Count,
                    x.InviteCode))
                .ToArray();
        }
    }

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

            if (!session.Participants.TryGetValue(normalizedDancerName, out var participant))
            {
                throw new SessionNotFoundException(sessionId);
            }

            if (participant.Token != Guid.Empty)
            {
                _tokenIndex.Remove(participant.Token);
            }

            session.Participants.Remove(normalizedDancerName);
        }
    }

    public IReadOnlyList<Participant> ListParticipants(Guid sessionId)
    {
        lock (_sync)
        {
            var session = GetSession(sessionId);
            return session.Participants.Values
                .OrderBy(x => x.DancerName, StringComparer.OrdinalIgnoreCase)
                .Select(x => new Participant(Guid.Empty, x.DancerName, x.Role, x.IsCoach, x.Token != Guid.Empty))
                .ToArray();
        }
    }

    public string GenerateInviteCode(Guid sessionId)
    {
        lock (_sync)
        {
            var session = GetSession(sessionId);
            EnsureSessionIsActive(session);

            var existingCodes = _sessions.Values
                .Where(s => s.InviteCode != null)
                .Select(s => s.InviteCode!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            string code;
            do
            {
                code = GenerateCode();
            }
            while (existingCodes.Contains(code));

            session.InviteCode = code;
            return code;
        }
    }

    public Guid JoinViaCode(string inviteCode, string dancerName, SessionRole role)
    {
        if (string.IsNullOrWhiteSpace(inviteCode))
        {
            throw new SessionValidationException("Invite code is required.");
        }

        var normalizedDancerName = RequireName(dancerName, "Dancer name");
        var normalizedRole = NormalizeRole(role);
        var normalizedCode = inviteCode.Trim().ToUpperInvariant();

        lock (_sync)
        {
            var session = _sessions.Values.FirstOrDefault(s =>
                string.Equals(s.InviteCode, normalizedCode, StringComparison.OrdinalIgnoreCase));

            if (session is null || session.Status == SessionStatus.Ended)
            {
                throw new SessionValidationException("Invite code not found or session has ended.");
            }

            // Return existing token if dancer already joined
            if (session.Participants.TryGetValue(normalizedDancerName, out var existing) && existing.Token != Guid.Empty)
            {
                return existing.Token;
            }

            var token = Guid.NewGuid();
            session.Participants[normalizedDancerName] = new ParticipantState(normalizedDancerName, normalizedRole, token);
            _tokenIndex[token] = (session.Id, normalizedDancerName);
            return token;
        }
    }

    public (Guid SessionId, string DancerName, bool IsCoach)? GetParticipantSession(Guid token)
    {
        lock (_sync)
        {
            if (_tokenIndex.TryGetValue(token, out var entry))
            {
                var session = GetSession(entry.SessionId);
                if (session.Participants.TryGetValue(entry.DancerName, out var participant))
                {
                    return (entry.SessionId, entry.DancerName, participant.IsCoach);
                }

                return (entry.SessionId, entry.DancerName, false);
            }

            return null;
        }
    }

    public void PromoteParticipant(Guid sessionId, string dancerName)
    {
        var normalizedDancerName = RequireName(dancerName, "Dancer name");

        lock (_sync)
        {
            var session = GetSession(sessionId);
            EnsureSessionIsActive(session);

            if (!session.Participants.TryGetValue(normalizedDancerName, out var participant))
            {
                throw new SessionNotFoundException(sessionId);
            }

            if (participant.Token == Guid.Empty)
            {
                throw new SessionValidationException(
                    $"Dancer '{normalizedDancerName}' cannot be promoted because they have no participant token. Only participants who joined via invite code can be promoted.");
            }

            session.Participants[normalizedDancerName] = participant with { IsCoach = true };
        }
    }

    public void DemoteParticipant(Guid sessionId, string dancerName)
    {
        var normalizedDancerName = RequireName(dancerName, "Dancer name");

        lock (_sync)
        {
            var session = GetSession(sessionId);
            EnsureSessionIsActive(session);

            if (!session.Participants.TryGetValue(normalizedDancerName, out var participant))
            {
                throw new SessionNotFoundException(sessionId);
            }

            session.Participants[normalizedDancerName] = participant with { IsCoach = false };
        }
    }

    private static string GenerateCode()
    {
        var chars = new char[6];
        for (var i = 0; i < 6; i++)
        {
            chars[i] = InviteCodeAlphabet[Random.Shared.Next(InviteCodeAlphabet.Length)];
        }
        return new string(chars);
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
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public Guid Id { get; }

        public string Name { get; }

        public DateTimeOffset CreatedAt { get; }

        public SessionStatus Status { get; set; } = SessionStatus.Active;

        public string? InviteCode { get; set; }

        public Dictionary<string, ParticipantState> Participants { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private sealed record ParticipantState(string DancerName, SessionRole Role, Guid Token = default, bool IsCoach = false);

    private enum SessionStatus
    {
        Active,
        Ended
    }
}
