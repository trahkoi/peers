using Peers.Persistence;
using Peers.Training.Dancers;

namespace Peers.Training.Sessions.Internal;

internal sealed class SqliteSessionService : ISessionService
{
    private static readonly char[] InviteCodeAlphabet =
        "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();

    private readonly PeersDbContext _db;
    private Microsoft.EntityFrameworkCore.DbSet<SessionEntity> Sessions => _db.Set<SessionEntity>();
    private Microsoft.EntityFrameworkCore.DbSet<ParticipantEntity> Participants => _db.Set<ParticipantEntity>();
    private Microsoft.EntityFrameworkCore.DbSet<DancerEntity> Dancers => _db.Set<DancerEntity>();

    public SqliteSessionService(PeersDbContext db)
    {
        _db = db;
    }

    public IReadOnlyList<SessionSummary> ListSessions()
    {
        return Sessions
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SessionSummary(
                s.Id,
                s.Name,
                s.IsEnded,
                s.Participants.Count,
                s.InviteCode))
            .ToArray();
    }

    public Guid CreateSession(string name)
    {
        var normalizedName = RequireName(name, "Session name");
        var session = new SessionEntity
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            CreatedAt = DateTime.UtcNow,
            IsEnded = false
        };

        Sessions.Add(session);
        _db.SaveChanges();
        return session.Id;
    }

    public void EndSession(Guid sessionId)
    {
        var session = GetSession(sessionId);

        if (session.IsEnded)
        {
            throw new SessionConflictException($"Session '{sessionId}' is already ended.");
        }

        session.IsEnded = true;
        _db.SaveChanges();
    }

    public void JoinSession(Guid sessionId, string dancerName, SessionRole role)
    {
        var normalizedDancerName = RequireName(dancerName, "Dancer name");
        var normalizedRole = NormalizeRole(role);

        var session = GetSession(sessionId);
        EnsureSessionIsActive(session);

        var exists = Participants.Any(p =>
            p.SessionId == sessionId &&
            p.DancerName.ToUpper() == normalizedDancerName.ToUpper());

        if (exists)
        {
            throw new SessionConflictException(
                $"Dancer '{normalizedDancerName}' is already participating in session '{sessionId}'.");
        }

        var dancer = ResolveOrCreateDancer(normalizedDancerName);

        Participants.Add(new ParticipantEntity
        {
            SessionId = sessionId,
            DancerId = dancer.Id,
            DancerName = normalizedDancerName,
            Role = normalizedRole
        });

        _db.SaveChanges();
    }

    public void LeaveSession(Guid sessionId, string dancerName)
    {
        var normalizedDancerName = RequireName(dancerName, "Dancer name");

        var session = GetSession(sessionId);
        EnsureSessionIsActive(session);

        var participant = Participants.FirstOrDefault(p =>
            p.SessionId == sessionId &&
            p.DancerName.ToUpper() == normalizedDancerName.ToUpper());

        if (participant is null)
        {
            throw new SessionNotFoundException(sessionId);
        }

        Participants.Remove(participant);
        _db.SaveChanges();
    }

    public IReadOnlyList<Participant> ListParticipants(Guid sessionId)
    {
        var session = GetSession(sessionId);

        return Participants
            .Where(p => p.SessionId == sessionId)
            .OrderBy(p => p.DancerName)
            .Select(p => new Participant(p.DancerName, p.Role))
            .ToArray();
    }

    public string GenerateInviteCode(Guid sessionId)
    {
        var session = GetSession(sessionId);
        EnsureSessionIsActive(session);

        var existingCodes = Sessions
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
        _db.SaveChanges();
        return code;
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

        var session = Sessions.FirstOrDefault(s =>
            s.InviteCode != null &&
            s.InviteCode.ToUpper() == normalizedCode);

        if (session is null || session.IsEnded)
        {
            throw new SessionValidationException("Invite code not found or session has ended.");
        }

        var existing = Participants.FirstOrDefault(p =>
            p.SessionId == session.Id &&
            p.DancerName.ToUpper() == normalizedDancerName.ToUpper());

        if (existing is not null && existing.Token is not null && existing.Token != Guid.Empty)
        {
            return existing.Token.Value;
        }

        var token = Guid.NewGuid();

        if (existing is not null)
        {
            existing.Token = token;
        }
        else
        {
            var dancer = ResolveOrCreateDancer(normalizedDancerName);

            Participants.Add(new ParticipantEntity
            {
                SessionId = session.Id,
                DancerId = dancer.Id,
                DancerName = normalizedDancerName,
                Role = normalizedRole,
                Token = token
            });
        }

        _db.SaveChanges();
        return token;
    }

    public (Guid SessionId, string DancerName)? GetParticipantSession(Guid token)
    {
        var participant = Participants.FirstOrDefault(p => p.Token == token);

        if (participant is null)
        {
            return null;
        }

        return (participant.SessionId, participant.DancerName);
    }

    private DancerEntity ResolveOrCreateDancer(string name)
    {
        var dancer = Dancers.FirstOrDefault(d => d.Name.ToUpper() == name.ToUpper());

        if (dancer is not null)
            return dancer;

        dancer = new DancerEntity { Id = Guid.NewGuid(), Name = name };
        Dancers.Add(dancer);
        return dancer;
    }

    private SessionEntity GetSession(Guid sessionId)
    {
        var session = Sessions.FirstOrDefault(s => s.Id == sessionId);

        if (session is null)
        {
            throw new SessionNotFoundException(sessionId);
        }

        return session;
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

    private static void EnsureSessionIsActive(SessionEntity session)
    {
        if (session.IsEnded)
        {
            throw new SessionConflictException($"Session '{session.Id}' has ended and can no longer be modified.");
        }
    }
}
