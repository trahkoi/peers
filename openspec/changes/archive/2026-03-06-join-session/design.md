## Context

The app is an ASP.NET Core Razor Pages app with an in-memory session service (`ISessionService` / `InMemorySessionService`). There is no authentication system. Sessions are currently managed by an admin/coach through the `/Sessions/Manage` page, which directly adds dancers by name. Participants have no self-service way to join — they are added by the admin.

The feature adds an invite-code-based flow: admins generate a short code, participants enter it on a public join page, and receive a personal token they can use to recover their session view.

## Goals / Non-Goals

**Goals:**
- Admin/coach can generate a human-shareable invite code for a session
- Participants join by entering the invite code and their dancer name + role
- Each participant receives a unique personal token scoped to the session
- Token can be used to re-open the session view (e.g., after closing the tab)
- No login required for participants

**Non-Goals:**
- Persistent storage (remains in-memory)
- Invite code expiry or one-time-use enforcement (codes are valid for the session's lifetime)
- Participant authentication beyond the personal token
- Multiple invite codes per session

## Decisions

### 1. Invite code format: short alphanumeric vs UUID

**Decision:** 6-character uppercase alphanumeric (e.g., `A3F9KX`), generated randomly from a safe alphabet (no ambiguous chars like 0/O, 1/I).

**Rationale:** The code is meant to be shared verbally or by chat (e.g., "the code is A3F9KX"). A UUID is too long. A 6-char code from 32 symbols gives ~1 billion combinations — sufficient for a small in-memory use case.

**Alternative considered:** UUID — simpler to generate but not human-friendly.

### 2. Personal token format

**Decision:** `Guid` (UUID v4), stored server-side mapped to `(sessionId, dancerName)`.

**Rationale:** Simple, unique, hard to guess. No need for signed JWTs since there's no auth infrastructure and the app is in-memory.

**Alternative considered:** Signed JWT — overkill for in-memory system; would add a dependency.

### 3. Token storage location

**Decision:** Token-to-participant mapping lives inside `InMemorySessionService`, as a new dictionary `_tokenIndex: Dictionary<Guid, (Guid sessionId, string dancerName)>`.

**Rationale:** Keeps all session state co-located. No architectural change needed.

### 4. Role selection

**Decision:** The participant selects their own role (Leader/Follower) on the join page.

**Rationale:** Consistent with how the admin currently adds participants (role is always specified). The admin can still override via the Manage page after join.

### 5. Participant session view

**Decision:** On successful join (or token re-entry), redirect the participant to a read-only session view page (`/sessions/{id}/view`) showing the session name and participant list.

**Rationale:** Participants don't need the full Manage page (which has admin actions). A separate read-only view is cleaner and avoids exposing admin controls.

### 6. Invite code uniqueness

**Decision:** Invite codes are globally unique at generation time (collision-checked against all active codes in memory).

**Rationale:** Prevents two sessions sharing a code, which would be confusing.

## Risks / Trade-offs

- **In-memory only** → All tokens and codes are lost on server restart. Participants lose their personal token on restart. Mitigation: acceptable for current scope; persistence is a future concern.
- **No code expiry** → Invite code stays valid until session ends. A leaked code lets anyone join. Mitigation: admin can end the session to invalidate; revocation is a future concern.
- **Token as sole identity** → If a participant loses their token URL, they cannot recover it (no email, no account). Mitigation: admin can view the session and re-add the participant; or re-join generates a new token.
- **Re-join behavior** → If a participant who already joined tries to join again with the same code and same name, the system should return their existing token rather than throwing a conflict error.

## Migration Plan

1. Extend `ISessionService` with new methods: `GenerateInviteCode`, `JoinViaCode`, `GetParticipantSession`
2. Extend `InMemorySessionService` with invite code and token state
3. Add new Razor pages: `/Sessions/Join` (public, enter code + name + role) and `/Sessions/{id}/View` (participant view)
4. Add invite code display + generate button to the Manage page
5. No breaking changes to existing `JoinSession`, `LeaveSession`, `EndSession` flows

## Open Questions

- Should re-joining (same code + same dancer name) regenerate the token or return the existing one? (Design above says: return existing token — avoids duplicate participant entries.)
- Should the participant view auto-refresh to show new joiners? (Out of scope for now — static page on load.)
