## Why

Training session participants currently have no self-service way to join a session — admins or coaches must manually manage access. A shareable session code gives participants a simple, secure entry point, while a personal participant token lets them recover their session context without full re-authentication.

## What Changes

- Session admins and coaches can generate an invite code for a training session
- Participants enter the invite code to join the session
- Upon joining, each participant receives a unique personal token
- The personal token can be used to re-open the session in the browser if they lose their session state (e.g., tab closed, browser refresh without persistent login)
- The invite code is reusable for the session's lifetime (or until revoked)
- The personal token is per-participant and scoped to that session

## Capabilities

### New Capabilities

- `session-invite-code`: Generate and manage a shareable invite code for a training session (admin/coach role required); code allows anyone with it to join the session
- `session-join`: Allow a participant to join a training session by entering a valid invite code; on success returns a personal participant token
- `participant-token`: A unique, per-participant token scoped to a session; can be used to restore session access in the browser without re-authenticating

### Modified Capabilities

## Impact

- `training` module: sessions domain — new fields on session entity (invite code, expiry); new participant-token entity
- Backend API: new endpoints for generating invite code, joining a session, and token-based session re-entry
- Frontend: join-session page (public, no login required); session re-entry flow using personal token
- No breaking changes to existing session management flows
