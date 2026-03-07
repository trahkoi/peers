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

    public bool IsCoach { get; private set; }

    public Guid Token { get; private set; }

    public bool TokenMissing { get; private set; }

    public bool TokenInvalid { get; private set; }

    public IActionResult OnGet(Guid token)
    {
        if (token == Guid.Empty)
        {
            TokenMissing = true;
            return Page();
        }

        var entry = _sessions.GetParticipantSession(token);
        if (entry is null)
        {
            TokenInvalid = true;
            return Page();
        }

        var session = _sessions.ListSessions().FirstOrDefault(s => s.SessionId == entry.Value.SessionId);
        if (session is null)
        {
            TokenInvalid = true;
            return Page();
        }

        Session = session;
        DancerName = entry.Value.DancerName;
        IsCoach = entry.Value.IsCoach;
        Token = token;
        Participants = _sessions.ListParticipants(session.SessionId);
        return Page();
    }
}
