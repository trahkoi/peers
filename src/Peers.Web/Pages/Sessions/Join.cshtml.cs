using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peers.Training.Sessions;

namespace Peers.Web.Pages.Sessions;

public class JoinModel : PageModel
{
    private readonly ISessionService _sessions;

    public JoinModel(ISessionService sessions)
    {
        _sessions = sessions;
    }

    [BindProperty]
    public string InviteCode { get; set; } = string.Empty;

    [BindProperty]
    public string DancerName { get; set; } = string.Empty;

    [BindProperty]
    public SessionRole Role { get; set; } = SessionRole.Leader;

    public void OnGet(string? code)
    {
        if (!string.IsNullOrWhiteSpace(code))
        {
            InviteCode = code.Trim().ToUpperInvariant();
        }
    }

    public IActionResult OnPostJoin()
    {
        try
        {
            var token = _sessions.JoinViaCode(InviteCode, DancerName, Role);
            return Redirect($"/sessions/view?token={token}");
        }
        catch (SessionValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Unexpected error while joining the session.");
            return Page();
        }
    }
}
