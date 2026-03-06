## Context

When a participant joins a session via invite code (`JoinViaCode`), the system creates a `ParticipantState` with a token and indexes it in `_tokenIndex[token] = (sessionId, dancerName)`. When a coach removes a participant via `LeaveSession`, only `session.Participants` is cleaned up — the `_tokenIndex` entry persists. This means the removed participant's token still resolves in `GetParticipantSession`, granting continued access to the session view.

## Goals / Non-Goals

**Goals:**
- Invalidate a participant's token when they are removed via `LeaveSession`
- Ensure the session view page correctly rejects invalidated tokens (already handles unknown tokens)

**Non-Goals:**
- Token invalidation when a session ends (existing behavior shows ended state, which is fine)
- Revocation of invite codes when participants are removed
- Notification to the removed participant

## Decisions

**1. Remove token from `_tokenIndex` inside `LeaveSession`**

The `LeaveSession` method already holds the lock and has access to the participant's state before removal. We look up the participant's `ParticipantState` to find their `Token`, then remove it from `_tokenIndex` before removing the participant from `session.Participants`.

Alternative considered: lazy invalidation in `GetParticipantSession` (check if participant still exists). Rejected because it leaves stale entries in `_tokenIndex` indefinitely and splits the cleanup logic across two methods.

**2. No changes to the View page or `GetParticipantSession`**

The View page already handles unknown tokens by showing "Session not found" with a link to the join page. Once the token is removed from `_tokenIndex`, this path activates naturally.

## Risks / Trade-offs

- **[Risk] Participant loses access with no explanation** → Acceptable for now; the view page shows a generic "not found" message. A future change could add a "you were removed" message, but that requires persisting removal state.
- **[Risk] Token cleanup only applies to `LeaveSession`, not `EndSession`** → By design. Ended sessions keep tokens valid so participants can still view the final roster. This is consistent with the existing `participant-token` spec.
