using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Peers.Web.Pages;

public class LoginModel : PageModel
{
    private readonly AdminCredentials _credentials;

    public LoginModel(IOptions<AdminCredentials> credentials)
    {
        _credentials = credentials.Value;
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public void OnGet(string? returnUrl) { }

    public async Task<IActionResult> OnPostLoginAsync(string? returnUrl)
    {
        if (Username != _credentials.Username || Password != _credentials.Password)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return Page();
        }

        var claims = new[] { new Claim(ClaimTypes.Name, Username) };
        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("Cookies", principal);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToPage("/Sessions/Index");
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToPage("/Login");
    }
}
