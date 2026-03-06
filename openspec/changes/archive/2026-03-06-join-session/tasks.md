## 1. Domain: Invite Code

- [x] 1.1 Add `InviteCode` (string) and `ParticipantTokens` dictionary (`Guid → (sessionId, dancerName)`) to `SessionState` in `InMemorySessionService`
- [x] 1.2 Add `GenerateInviteCode(Guid sessionId): string` to `ISessionService`
- [x] 1.3 Implement `GenerateInviteCode` in `InMemorySessionService`: generate a 6-char uppercase alphanumeric code (excluding 0, O, 1, I), check global uniqueness across all sessions, retry on collision, store on session, replace previous code
- [x] 1.4 Throw `SessionConflictException` if session has ended when generating invite code

## 2. Domain: Join via Code & Participant Token

- [x] 2.1 Add `JoinViaCode(string inviteCode, string dancerName, SessionRole role): Guid` to `ISessionService`
- [x] 2.2 Implement `JoinViaCode` in `InMemorySessionService`: look up session by invite code, validate session is active, add participant (or return existing token if dancer name already present), generate and store a UUID token mapped to `(sessionId, dancerName)`
- [x] 2.3 Add `GetParticipantSession(Guid token): (Guid sessionId, string dancerName)?` to `ISessionService`
- [x] 2.4 Implement `GetParticipantSession` in `InMemorySessionService`: look up token in global token index, return `null` if not found

## 3. Web: Manage Page — Invite Code

- [x] 3.1 Add `InviteCode` property to `ManageModel` and populate it from the session state
- [x] 3.2 Add `OnPostGenerateCode(Guid id)` handler in `ManageModel` calling `GenerateInviteCode`
- [x] 3.3 Update `Manage.cshtml` to display the current invite code (if any) and a "Generate invite code" / "Regenerate" button

## 4. Web: Join Page

- [x] 4.1 Create `Pages/Sessions/Join.cshtml.cs` — public page accepting `code` (query param), `DancerName` (bound), `Role` (bound)
- [x] 4.2 Implement `OnGet` to pre-fill the invite code from the `code` query parameter
- [x] 4.3 Implement `OnPostJoin` calling `JoinViaCode`; on success redirect to `/sessions/view?token={token}`; on failure re-display form with error
- [x] 4.4 Create `Pages/Sessions/Join.cshtml` with the join form (invite code field, dancer name field, role selector, submit button)

## 5. Web: Participant Session View Page

- [x] 5.1 Create `Pages/Sessions/View.cshtml.cs` — public page accepting `token` (query param); resolve session and participant via `GetParticipantSession`; load session summary and participant list
- [x] 5.2 Handle unknown token: display error message "Session not found. Your link may have expired." with a link to the join page
- [x] 5.3 Create `Pages/Sessions/View.cshtml` showing session name, status (Active/Ended), and participant roster (read-only)
