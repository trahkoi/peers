## 1. Data Model

- [x] 1.1 Add `IsCoach` boolean column to `ParticipantEntity` (default false)
- [x] 1.2 Add `IsCoach` property to the `Participant` record
- [x] 1.3 Create EF Core migration for the new column

## 2. Service Layer

- [x] 2.1 Add `PromoteParticipant(Guid sessionId, string dancerName)` to `ISessionService`
- [x] 2.2 Add `DemoteParticipant(Guid sessionId, string dancerName)` to `ISessionService`
- [x] 2.3 Implement promote/demote in `SqliteSessionService` (validate active session, idempotent)
- [x] 2.4 Implement promote/demote in `InMemorySessionService`
- [x] 2.5 Extend `GetParticipantSession` return type to include `IsCoach` flag

## 3. Manage Page — Token Access

- [x] 3.1 Update `ManageModel` to accept an optional `token` query parameter
- [x] 3.2 Allow access when token maps to a promoted participant (bypass `[Authorize]` for token path)
- [x] 3.3 Redirect non-promoted token users to the View page
- [x] 3.4 Track whether current user is admin coach vs. promoted participant
- [x] 3.5 Hide restricted actions (end session, invite code, promote/demote) for promoted participants in the Razor view

## 4. Manage Page — Promote/Demote UI

- [x] 4.1 Add promote/demote buttons next to each participant on the Manage page (admin coach only)
- [x] 4.2 Add `OnPostPromote` and `OnPostDemote` handlers to `ManageModel`
- [x] 4.3 Show promoted status indicator on participant list

## 5. View Page — Manage Link

- [x] 5.1 Extend `ViewModel` to include `IsCoach` flag from participant data
- [x] 5.2 Show "Manage Session" link on View page when participant is promoted

## 6. Tests

- [x] 6.1 Unit tests for promote/demote service methods (happy path, ended session, idempotent)
- [x] 6.2 Unit tests for Manage page token access (promoted granted, non-promoted redirected)
- [x] 6.3 Unit tests for restricted action enforcement (promoted participant cannot end/invite/promote)
