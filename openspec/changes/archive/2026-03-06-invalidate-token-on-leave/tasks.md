## 1. Core Implementation

- [x] 1.1 In `InMemorySessionService.LeaveSession`, look up the participant's `ParticipantState` before removal, and if their `Token` is not `Guid.Empty`, remove it from `_tokenIndex`
- [x] 1.2 Verify that `GetParticipantSession` returns `null` for a removed participant's token (no code change expected — just confirm the flow)

## 2. Tests

- [x] 2.1 Add test: token is invalidated after participant is removed via `LeaveSession` — `GetParticipantSession` returns null for the old token
- [x] 2.2 Add test: removed participant can re-join via invite code and receives a new token that is valid
- [x] 2.3 Add test: removing a participant who joined via `JoinSession` (no token) does not fail or affect `_tokenIndex`
