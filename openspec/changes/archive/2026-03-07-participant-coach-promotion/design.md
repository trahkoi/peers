## Context

The Manage page (`/Sessions/Manage`) is protected by `[Authorize]` and requires admin login. Participants access only the read-only View page via their personal token. Coaches want to delegate session management to trusted dancers without sharing admin credentials.

The `Participant` record currently holds `DancerId`, `DancerName`, and `Role` (Leader/Follower). There is no concept of elevated permissions for participants.

## Goals / Non-Goals

**Goals:**
- Allow coaches to promote a participant to manage a specific session
- Promoted participants can add/remove dancers and generate spotlight pairings
- Promotion is session-scoped and uses the existing token mechanism
- Coaches can revoke promotion at any time

**Non-Goals:**
- Multi-level permission system or fine-grained capability control
- Persistent coach accounts for participants across sessions
- Allowing promoted participants to end sessions or manage invite codes

## Decisions

### 1. Boolean `IsCoach` flag on Participant vs. separate permission model

**Decision**: Add a boolean `IsCoach` property to `ParticipantEntity` and `Participant`.

**Rationale**: The only distinction is "can manage or not." A full permission model is over-engineering for a binary capability. The existing `SessionRole` (Leader/Follower) represents dance role and is orthogonal to management permissions.

**Alternative considered**: Reusing `SessionRole` with a new `Coach` value — rejected because dance role and management capability are independent (a promoted leader is still a leader for pairing purposes).

### 2. Token-based Manage page access vs. separate auth flow

**Decision**: Extend the Manage page to accept a `token` query parameter. If a valid token maps to a promoted participant, grant access without `[Authorize]`.

**Rationale**: Participants already authenticate via token for the View page. Reusing this pattern keeps the UX consistent — no login required. The token is already a secret that proves identity.

**Alternative considered**: Issuing a separate management token — rejected as unnecessary complexity. The participant token already uniquely identifies a participant in a session.

### 3. Scope of promoted participant permissions

**Decision**: Promoted participants can: add/remove dancers, generate spotlight pairings. They cannot: end the session, generate invite codes, promote/demote other participants.

**Rationale**: These are the "hands-on" management tasks a coach delegates while dancing. Session lifecycle and trust decisions (invite codes, promotions) stay with the admin coach.

## Risks / Trade-offs

- **Token exposure grants management access** → Acceptable because tokens are already secret credentials. Same risk profile as current View page tokens, just with more capability for promoted users.
- **No audit trail of who made changes** → Out of scope for now. The app doesn't track who performed actions currently either.
- **Promoted participant removed from session loses management access** → By design. Removing a participant invalidates their token, which also revokes management access.
