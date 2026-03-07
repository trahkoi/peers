using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peers.Spotlights;
using Peers.Training.Sessions;

namespace Peers.Web.Pages.Sessions;

public class ManageModel : PageModel
{
    private readonly ISessionService _sessions;
    private readonly ISpotlightService? _spotlights;

    public ManageModel(ISessionService sessions, ISpotlightService? spotlights = null)
    {
        _sessions = sessions;
        _spotlights = spotlights;
    }

    public Guid SessionId { get; private set; }

    public SessionSummary? Session { get; private set; }

    public IReadOnlyList<Participant> Participants { get; private set; } = [];

    public IReadOnlyList<SpotlightRound> SpotlightRounds { get; private set; } = [];

    public bool IsAdminCoach { get; private set; }

    public Guid? ParticipantToken { get; private set; }

    [BindProperty]
    public string DancerName { get; set; } = string.Empty;

    [BindProperty]
    public SessionRole Role { get; set; } = SessionRole.Leader;

    [TempData]
    public string? FlashMessage { get; set; }

    [TempData]
    public string? FlashTone { get; set; }

    public IActionResult OnGet(Guid id, Guid? token)
    {
        var authResult = Authorize(id, token);
        if (authResult is not null) return authResult;

        return LoadPageState(SessionId) ? Page() : RedirectToPage("/Sessions/Index");
    }

    public IActionResult OnPostJoin(Guid id, Guid? token)
    {
        var authResult = Authorize(id, token);
        if (authResult is not null) return authResult;

        try
        {
            _sessions.JoinSession(SessionId, DancerName, Role);
            FlashMessage = $"Added {DancerName.Trim()} as {Role.ToString().ToLowerInvariant()}.";
            FlashTone = "success";
            return RedirectToManage();
        }
        catch (Exception ex)
        {
            return HandleSessionException(ex, nameof(DancerName), SessionId);
        }
    }

    public IActionResult OnPostLeave(Guid id, string dancerName, Guid? token)
    {
        var authResult = Authorize(id, token);
        if (authResult is not null) return authResult;

        try
        {
            _sessions.LeaveSession(SessionId, dancerName);
            FlashMessage = $"{dancerName.Trim()} left the session.";
            FlashTone = "success";
            return RedirectToManage();
        }
        catch (Exception ex)
        {
            return HandleSessionException(ex, string.Empty, SessionId);
        }
    }

    public IActionResult OnPostEnd(Guid id, Guid? token)
    {
        var authResult = Authorize(id, token);
        if (authResult is not null) return authResult;

        if (!IsAdminCoach) return Forbid();

        try
        {
            _sessions.EndSession(SessionId);
            FlashMessage = "Session ended. Attendance is now locked.";
            FlashTone = "info";
            return RedirectToManage();
        }
        catch (Exception ex)
        {
            return HandleSessionException(ex, string.Empty, SessionId);
        }
    }

    public IActionResult OnPostGenerateCode(Guid id, Guid? token)
    {
        var authResult = Authorize(id, token);
        if (authResult is not null) return authResult;

        if (!IsAdminCoach) return Forbid();

        try
        {
            _sessions.GenerateInviteCode(SessionId);
            FlashMessage = "Invite code generated.";
            FlashTone = "success";
            return RedirectToManage();
        }
        catch (Exception ex)
        {
            return HandleSessionException(ex, string.Empty, SessionId);
        }
    }

    public IActionResult OnPostGenerateSpotlight(Guid id, Guid? token)
    {
        var authResult = Authorize(id, token);
        if (authResult is not null) return authResult;

        if (_spotlights is null)
            return RedirectToManage();

        try
        {
            var participants = _sessions.ListParticipants(SessionId);
            var leaders = participants.Where(p => p.Role == SessionRole.Leader).Select(p => p.DancerId).ToList();
            var followers = participants.Where(p => p.Role == SessionRole.Follower).Select(p => p.DancerId).ToList();
            _spotlights.GenerateRound(SessionId, leaders, followers);
            FlashMessage = "Spotlight pairings generated.";
            FlashTone = "success";
            return RedirectToManage();
        }
        catch (Exception ex)
        {
            return HandleSessionException(ex, string.Empty, SessionId);
        }
    }

    public IActionResult OnPostDeleteSpotlight(Guid id, Guid roundId, Guid? token)
    {
        var authResult = Authorize(id, token);
        if (authResult is not null) return authResult;

        if (_spotlights is null)
            return RedirectToManage();

        _spotlights.DeleteRound(roundId);
        FlashMessage = "Spotlight round deleted.";
        FlashTone = "success";
        return RedirectToManage();
    }

    public IActionResult OnPostPromote(Guid id, string dancerName, Guid? token)
    {
        var authResult = Authorize(id, token);
        if (authResult is not null) return authResult;

        if (!IsAdminCoach) return Forbid();

        try
        {
            _sessions.PromoteParticipant(SessionId, dancerName);
            FlashMessage = $"{dancerName.Trim()} promoted to session coach.";
            FlashTone = "success";
            return RedirectToManage();
        }
        catch (Exception ex)
        {
            return HandleSessionException(ex, string.Empty, SessionId);
        }
    }

    public IActionResult OnPostDemote(Guid id, string dancerName, Guid? token)
    {
        var authResult = Authorize(id, token);
        if (authResult is not null) return authResult;

        if (!IsAdminCoach) return Forbid();

        try
        {
            _sessions.DemoteParticipant(SessionId, dancerName);
            FlashMessage = $"{dancerName.Trim()} demoted to regular participant.";
            FlashTone = "success";
            return RedirectToManage();
        }
        catch (Exception ex)
        {
            return HandleSessionException(ex, string.Empty, SessionId);
        }
    }

    private IActionResult? Authorize(Guid id, Guid? token)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            IsAdminCoach = true;
            SessionId = id;
            return null;
        }

        if (token is { } t && t != Guid.Empty)
        {
            var entry = _sessions.GetParticipantSession(t);
            if (entry is not null && entry.Value.IsCoach)
            {
                IsAdminCoach = false;
                SessionId = entry.Value.SessionId;
                ParticipantToken = t;
                return null;
            }

            if (entry is not null)
            {
                return RedirectToPage("/Sessions/View", new { token = t });
            }
        }

        return Challenge();
    }

    private IActionResult RedirectToManage()
    {
        if (ParticipantToken is { } t)
            return RedirectToPage(new { id = SessionId, token = t });

        return RedirectToPage(new { id = SessionId });
    }

    private IActionResult HandleSessionException(Exception ex, string field, Guid sessionId)
    {
        switch (ex)
        {
            case SessionNotFoundException notFound:
                FlashMessage = notFound.Message;
                FlashTone = "error";
                return RedirectToPage("/Sessions/Index");
            case SessionValidationException:
            case SessionConflictException:
                ModelState.AddModelError(field, ex.Message);
                break;
            default:
                ModelState.AddModelError(string.Empty, "Unexpected error while updating the session.");
                break;
        }

        if (!LoadPageState(sessionId))
        {
            return RedirectToPage("/Sessions/Index");
        }

        return Page();
    }

    private bool LoadPageState(Guid sessionId)
    {
        var session = _sessions.ListSessions().FirstOrDefault(x => x.SessionId == sessionId);
        if (session is null)
        {
            FlashMessage = $"Session '{sessionId}' was not found.";
            FlashTone = "error";
            return false;
        }

        SessionId = sessionId;
        Session = session;
        Participants = _sessions.ListParticipants(sessionId);
        SpotlightRounds = _spotlights?.GetRounds(sessionId) ?? [];
        return true;
    }
}
