## 1. Configuration

- [x] 1.1 Add `AdminCredentials` section to `appsettings.json` with `Username` and `Password` fields (use placeholder values)
- [x] 1.2 Add a corresponding `AdminCredentials` options class in `Peers.Web` and bind it in `Program.cs`

## 2. Auth Middleware

- [x] 2.1 Register ASP.NET Core cookie authentication in `Program.cs` (`AddAuthentication().AddCookie()`) with login path `/login` and session-scoped cookie (no persistence)
- [x] 2.2 Add `app.UseAuthentication()` and `app.UseAuthorization()` to the middleware pipeline in the correct order

## 3. Login Page

- [x] 3.1 Create `Pages/Login.cshtml.cs` with `OnGet(string? returnUrl)` and `OnPostLogin(string? returnUrl)` handlers
- [x] 3.2 `OnPostLogin` validates credentials against `AdminCredentials` options; on success calls `HttpContext.SignInAsync` and redirects to `returnUrl` (validated local) or `/sessions`; on failure adds a model error
- [x] 3.3 Create `Pages/Login.cshtml` with username/password form and error display

## 4. Logout

- [x] 4.1 Add a `OnPostLogout` handler (or dedicated `Logout.cshtml.cs`) that calls `HttpContext.SignOutAsync` and redirects to `/login`

## 5. Protect Admin Pages

- [x] 5.1 Add `[Authorize]` attribute to `IndexModel` (`/sessions`)
- [x] 5.2 Add `[Authorize]` attribute to `NewModel` (`/sessions/new`)
- [x] 5.3 Add `[Authorize]` attribute to `ManageModel` (`/sessions/{id}`)
- [x] 5.4 Confirm `JoinModel` and `ViewModel` have no `[Authorize]` — they must remain public

## 6. Layout

- [x] 6.1 Add a "Log out" form/button to `_Layout.cshtml` that is only visible when the user is authenticated (`User.Identity.IsAuthenticated`)
