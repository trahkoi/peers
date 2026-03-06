## ADDED Requirements

### Requirement: Session data persists across application restarts
The system SHALL store all session and participant data in a SQLite database so that data survives application restarts and redeploys. All operations defined by `ISessionService` SHALL behave identically whether backed by the in-memory or SQLite implementation.

#### Scenario: Sessions survive restart
- **WHEN** a session with participants exists and the application restarts
- **THEN** the session, its participants, invite code, and status SHALL be available after restart
- **THEN** all participant tokens SHALL remain valid

#### Scenario: Ended sessions are preserved
- **WHEN** a session has been ended and the application restarts
- **THEN** the session SHALL still appear in the session list with `IsEnded = true`
- **THEN** the session's participant roster SHALL be intact

### Requirement: SQLite implementation satisfies the ISessionService contract
The `SqliteSessionService` SHALL implement `ISessionService` and pass all existing behavioral specifications. The implementation SHALL use EF Core with a SQLite provider.

#### Scenario: Create session persists to database
- **WHEN** `CreateSession(name)` is called
- **THEN** a new session row is written to the database with the given name, a generated Id, and `IsEnded = false`

#### Scenario: Join session persists participant
- **WHEN** `JoinSession(sessionId, dancerName, role)` is called for an active session
- **THEN** a new participant row is written with the session foreign key, dancer name, and role

#### Scenario: Token lookup queries the database
- **WHEN** `GetParticipantSession(token)` is called with a valid token
- **THEN** the system queries the participants table by token and returns the session ID and dancer name

#### Scenario: Invite code uniqueness enforced by database
- **WHEN** `GenerateInviteCode(sessionId)` is called
- **THEN** the generated code is checked against existing codes in the database
- **THEN** the code is stored on the session row

### Requirement: Application selects persistence mode via configuration
The system SHALL use SQLite persistence when a `Sessions` connection string is configured. When no connection string is present, the system SHALL fall back to the in-memory implementation.

#### Scenario: Connection string present selects SQLite
- **WHEN** the application starts with a `ConnectionStrings:Sessions` configuration value
- **THEN** the system registers `SqliteSessionService` as the `ISessionService` implementation
- **THEN** EF Core migrations are applied automatically on startup

#### Scenario: No connection string falls back to in-memory
- **WHEN** the application starts without a `ConnectionStrings:Sessions` configuration value
- **THEN** the system registers `InMemorySessionService` as the `ISessionService` implementation
- **THEN** no database file is created

### Requirement: Database schema supports sessions and participants
The system SHALL define an EF Core model with `Sessions` and `Participants` tables. The schema SHALL enforce referential integrity and uniqueness constraints at the database level.

#### Scenario: Participant dancer name is unique per session
- **WHEN** a participant with the same dancer name is added to the same session twice
- **THEN** the database SHALL reject the duplicate via a unique constraint on (SessionId, DancerName)

#### Scenario: Token is unique across all participants
- **WHEN** a participant token is stored
- **THEN** the database SHALL enforce uniqueness on the Token column (for non-null values)

#### Scenario: Invite code is unique across sessions
- **WHEN** an invite code is stored on a session
- **THEN** the database SHALL enforce uniqueness on the InviteCode column (for non-null values)

#### Scenario: Participants reference a valid session
- **WHEN** a participant row exists
- **THEN** the SessionId foreign key SHALL reference an existing session
- **THEN** deleting a session SHALL cascade to its participants
