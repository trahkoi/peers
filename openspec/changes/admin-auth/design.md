## Context

The app is an ASP.NET Core Razor Pages app with no authentication. All pages — including session creation and management — are publicly accessible. Participants join via invite code (no login required by design). The goal is to add a lightweight login wall for the admin/coach side only, without introducing a user database or a heavy identity framework.

## Goals / Non-Goals

**Goals:**
- Protect session management pages behind a login wall
- Use ASP.NET Core's built-in cookie authentication (no external packages)
- Credentials stored in `appsettings.json` / environment variables (single admin account)
- Participant pages (`/sessions/join`, `/sessions/view`) remain fully public

**Non-Goals:**
- Multiple admin accounts or role management
- Password hashing (acceptable for a single-user dev/demo app; can be revisited)
- "Remember me" / persistent login beyond the browser session
- Account registration or password reset flows
- ASP.NET Core Identity (overkill for a single hardcoded credential)

## Decisions

### 1. Auth mechanism: cookie authentication vs. other approaches

**Decision:** ASP.NET Core cookie authentication (`AddAuthentication().AddCookie()`).

**Rationale:** Ships in-box, integrates natively with Razor Pages `[Authorize]` attribute and `RequireAuthorization()`, no external packages. Session-scoped cookie (no persistence) is appropriate for a coach who logs in at the start of class.

**Alternative considered:** HTTP Basic Auth — browser UX is poor (native dialog, hard to style, no logout). JWT — stateless but adds complexity with no benefit here.

### 2. Credential storage: appsettings vs. hardcoded

**Decision:** `appsettings.json` with an `AdminCredentials` section (`{ Username, Password }`).

**Rationale:** Easy to override per environment via `appsettings.Production.json` or environment variables (`AdminCredentials__Password`). Keeps credentials out of source code.

**Alternative considered:** Hardcoded in code — simpler but inflexible and leaks credentials into version control.

### 3. Password comparison: plaintext vs. hashed

**Decision:** Plaintext comparison for now, with a note to hash in the future.

**Rationale:** This is a single-admin, in-memory demo app with no user database. Hashing adds ceremony without meaningful security gain at this stage. The password lives in config, not a shared DB.

**Risk:** If config is committed to a public repo, the password is exposed. Mitigation: document that `appsettings.json` containing the password should be gitignored or the password set via environment variable.

### 4. Which pages to protect

**Decision:** Protect `/sessions` (list), `/sessions/new` (create), and `/sessions/{id}` (manage) with `[Authorize]`. Leave `/sessions/join` and `/sessions/view` public.

**Rationale:** Admins manage sessions; participants join without accounts. This is the minimal protection surface.

### 5. Redirect behaviour

**Decision:** Unauthenticated requests to protected pages redirect to `/login?returnUrl=<path>`. After successful login, redirect to `returnUrl` (validated to be local).

**Rationale:** Standard ASP.NET Core cookie auth behavior. Coaches naturally land on the page they were trying to reach.

## Risks / Trade-offs

- **Plaintext password in config** → Acceptable for demo/dev scope; document the environment variable override path for production-like deployments.
- **Single admin account** → Sufficient for a coach running their own instance; not suitable for multi-coach orgs. Future: add multiple accounts to config array.
- **In-memory auth state** → Cookie is browser-session-scoped; server restart doesn't invalidate it (cookie validation checks the data protection key, which also resets on restart — so restart effectively logs out). Acceptable.
- **No CSRF protection on login form** → ASP.NET Core Razor Pages includes anti-forgery tokens by default; no extra work needed.

## Migration Plan

1. Add `AdminCredentials` section to `appsettings.json`
2. Register cookie authentication in `Program.cs`
3. Add `[Authorize]` to protected page models
4. Add `Login` Razor page (GET + POST handlers) and `Logout` handler
5. Add "Log out" link to the shared layout (visible only when authenticated)
6. No data migration needed — in-memory, no persistence
