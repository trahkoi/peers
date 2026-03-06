## Why

When a coach removes a participant from a session via the manage page (`LeaveSession`), the participant's personal token remains valid in the `_tokenIndex`. The kicked participant can still access the session view page using their token, seeing the roster and session state as if they were still a member. The token should be invalidated so removed participants lose access immediately.

## What Changes

- `LeaveSession` will clean up the participant's token from the token index when removing them from a session
- The session view page will treat an invalidated token the same as an unknown token (show "Session not found" error with a link to the join page)
- No new APIs or endpoints — this is a fix to existing behavior

## Capabilities

### New Capabilities

_(none)_

### Modified Capabilities

- `participant-token`: Add requirement that a participant's token is invalidated when they are removed from a session via `LeaveSession`

## Impact

- `InMemorySessionService.LeaveSession` — must remove token from `_tokenIndex`
- `InMemorySessionService` internal state — `ParticipantState` must be queryable by dancer name to find the associated token
- Existing tests in `SessionServiceSpecificationTests` and `SessionApiBoundaryTests` — new test cases needed for token invalidation on leave
- Session view page behavior unchanged (already handles unknown tokens correctly)
