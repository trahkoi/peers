using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peers.Training.Sessions;

namespace Peers.Web.Pages.Sessions;

public class ManageModel : PageModel
{
    private readonly ISessionService _sessions;

    public ManageModel(ISessionService sessions)
    {
        _sessions = sessions;
    }

    public Guid SessionId { get; private set; }

    public SessionSummary? Session { get; private set; }

    public IReadOnlyList<Participant> Participants { get; private set; } = [];

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
        return true;
    }
}
