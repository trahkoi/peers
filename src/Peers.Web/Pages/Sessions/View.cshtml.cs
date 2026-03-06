using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peers.Training.Sessions;

namespace Peers.Web.Pages.Sessions;

public class ViewModel : PageModel
{
    private readonly ISessionService _sessions;

    public ViewModel(ISessionService sessions)
    {
        _sessions = sessions;
    }

    public SessionSummary? Session { get; private set; }

    public IReadOnlyList<Participant> Participants { get; private set; } = [];

    public string DancerName { get; private set; } = string.Empty;

    public bool TokenNotFound { get; private set; }

    public IActionResult OnGet(Guid token)
    {
        if (token == Guid.Empty)
        {
            TokenNotFound = true;
            return Page();
        }

        var entry = _sessions.GetParticipantSession(token);
        if (entry is null)
        {
            TokenNotFound = true;
            return Page();
        }

        var session = _sessions.ListSessions().FirstOrDefault(s => s.SessionId == entry.Value.SessionId);
        if (session is null)
        {
            TokenNotFound = true;
            return Page();
        }

        Session = session;
        DancerName = entry.Value.DancerName;
        Participants = _sessions.ListParticipants(session.SessionId);
        return Page();
    }
}
