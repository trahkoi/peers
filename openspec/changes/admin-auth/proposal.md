## Why

The session management pages (create, manage, end session) are currently open to anyone — there is no concept of an admin or coach identity. Any visitor can create or destroy sessions. Adding a login wall for admin/coach actions prevents unintended or malicious interference with live training sessions.

## What Changes

- A login page (`/login`) is added; unauthenticated users are redirected here when accessing protected pages
- Admin credentials (username + password) are configured via app settings — no database or user management required
- ASP.NET Core cookie authentication protects the session management pages
- The participant-facing pages (`/sessions/join`, `/sessions/view`) remain fully public — no login required
- A logout action is added so admins can sign out

## Capabilities

### New Capabilities

- `admin-login`: A credential-based login flow for the admin/coach role; successful login issues an auth cookie that grants access to session management pages

### Modified Capabilities

- `session-invite-code`: Generating an invite code now requires an authenticated admin/coach (previously unprotected)
- `session-join`: No change to requirements — join page remains public
- `participant-token`: No change to requirements — view page remains public

## Impact

- `Peers.Web` — add ASP.NET Core cookie authentication middleware; protect `/sessions`, `/sessions/new`, `/sessions/{id}` pages; add `/login` and `/logout` pages
- `appsettings.json` — new `AdminCredentials` section (username + hashed or plaintext password for dev)
- No changes to `Peers.Training` service layer
- No new external dependencies (ASP.NET Core auth ships in-box)
