using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peers.Training.Sessions;

namespace Peers.Web.Pages.Sessions;

public class NewModel : PageModel
{
    private readonly ISessionService _sessions;

    public NewModel(ISessionService sessions)
    {
        _sessions = sessions;
    }

    [BindProperty]
    public string SessionName { get; set; } = string.Empty;

    [TempData]
    public string? FlashMessage { get; set; }

    [TempData]
    public string? FlashTone { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        try
        {
            var sessionId = _sessions.CreateSession(SessionName);
            FlashMessage = "Session created. Start managing attendance.";
            FlashTone = "success";
            return RedirectToPage("/Sessions/Manage", new { id = sessionId });
        }
        catch (SessionValidationException ex)
        {
            ModelState.AddModelError(nameof(SessionName), ex.Message);
            return Page();
        }
    }
}
