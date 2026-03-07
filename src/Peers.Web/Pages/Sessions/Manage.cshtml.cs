using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peers.Spotlights;
using Peers.Training.Sessions;

namespace Peers.Web.Pages.Sessions;

[Authorize]
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

    [BindProperty]
    public string DancerName { get; set; } = string.Empty;

    [BindProperty]
    public SessionRole Role { get; set; } = SessionRole.Leader;

    [TempData]
    public string? FlashMessage { get; set; }

    [TempData]
    public string? FlashTone { get; set; }

    public IActionResult OnGet(Guid id)
    {
        return LoadPageState(id) ? Page() : RedirectToPage("/Sessions/Index");
    }

    public IActionResult OnPostJoin(Guid id)
    {
        try
        {
            _sessions.JoinSession(id, DancerName, Role);
            FlashMessage = $"Added {DancerName.Trim()} as {Role.ToString().ToLowerInvariant()}.";
            FlashTone = "success";
            return RedirectToPage(new { id });
        }
        catch (Exception ex)
        {
            return HandleSessionException(ex, nameof(DancerName), id);
        }
    }

    public IActionResult OnPostLeave(Guid id, string dancerName)
    {
        try
        {
            _sessions.LeaveSession(id, dancerName);
            FlashMessage = $"{dancerName.Trim()} left the session.";
            FlashTone = "success";
            return RedirectToPage(new { id });
        }
        catch (Exception ex)
        {
            return HandleSessionException(ex, string.Empty, id);
        }
    }

    public IActionResult OnPostEnd(Guid id)
    {
        try
        {
            _sessions.EndSession(id);
            FlashMessage = "Session ended. Attendance is now locked.";
            FlashTone = "info";
            return RedirectToPage(new { id });
        }
        catch (Exception ex)
        {
            return HandleSessionException(ex, string.Empty, id);
        }
    }

    public IActionResult OnPostGenerateCode(Guid id)
    {
        try
        {
            _sessions.GenerateInviteCode(id);
            FlashMessage = "Invite code generated.";
            FlashTone = "success";
            return RedirectToPage(new { id });
        }
        catch (Exception ex)
        {
            return HandleSessionException(ex, string.Empty, id);
        }
    }

    public IActionResult OnPostGenerateSpotlight(Guid id)
    {
        if (_spotlights is null)
            return RedirectToPage(new { id });

        try
        {
            var participants = _sessions.ListParticipants(id);
            var leaders = participants.Where(p => p.Role == SessionRole.Leader).Select(p => p.DancerId).ToList();
            var followers = participants.Where(p => p.Role == SessionRole.Follower).Select(p => p.DancerId).ToList();
            _spotlights.GenerateRound(id, leaders, followers);
            FlashMessage = "Spotlight pairings generated.";
            FlashTone = "success";
            return RedirectToPage(new { id });
        }
        catch (Exception ex)
        {
            return HandleSessionException(ex, string.Empty, id);
        }
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
