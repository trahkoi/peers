using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peers.Training.Sessions;

namespace Peers.Web.Pages.Sessions;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ISessionService _sessions;

    public IndexModel(ISessionService sessions)
    {
        _sessions = sessions;
    }

    public IReadOnlyList<SessionSummary> Sessions { get; private set; } = [];

    [TempData]
    public string? FlashMessage { get; set; }

    [TempData]
    public string? FlashTone { get; set; }

    public void OnGet()
    {
        Sessions = _sessions.ListSessions();
    }
}
