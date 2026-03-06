## Context

The app uses `InMemorySessionService` as a singleton — all state lives in dictionaries behind a lock. The `ISessionService` interface is the only access point; Razor Pages consume it via DI. There are no external dependencies today. The app runs on Azure App Service B1 (single instance, Linux).

The data model is small: sessions and participants, plus a token index (token → session+dancer). The schema is expected to grow over time.

## Goals / Non-Goals

**Goals:**
- Session and participant data survives app restarts and redeploys
- EF Core manages the schema so future migrations are straightforward
- Both in-memory and SQLite implementations pass the same behavioral test suite
- SQLite file persists on Azure App Service across restarts

**Non-Goals:**
- Multi-instance / horizontal scaling (single SQLite file, single app instance)
- Session cleanup, archival, or TTL
- Migrating existing in-memory data (restart starts fresh anyway)
- Switching token format (opaque Guid stays)

## Decisions

### 1. EF Core DbContext and entity model

**Decision:** Add a `SessionDbContext` in `Peers.Training` with two entity classes: `SessionEntity` and `ParticipantEntity`.

**Rationale:** The domain records (`SessionSummary`, `Participant`) are read-only projections — not suitable as EF entities. Separate entity classes keep the persistence model decoupled from the public API. The entities map directly to the private `SessionState`/`ParticipantState` that already exist inside `InMemorySessionService`.

**Schema:**

```
Sessions                          Participants
┌──────────────┬───────────┐      ┌──────────────┬───────────┐
│ Id           │ Guid (PK) │      │ Id           │ int (PK)  │
│ Name         │ text      │      │ SessionId    │ Guid (FK) │
│ CreatedAt    │ datetime  │      │ DancerName   │ text      │
│ IsEnded      │ bool      │      │ Role         │ int       │
│ InviteCode   │ text?     │      │ Token        │ Guid?     │
└──────────────┴───────────┘      └──────────────┴───────────┘

Indexes:
- Participants: unique (SessionId, DancerName)
- Participants: unique (Token) WHERE Token IS NOT NULL
- Sessions: unique (InviteCode) WHERE InviteCode IS NOT NULL
```

**Alternative considered:** Reuse the public `Participant` / `SessionSummary` records as entities. Rejected because they lack mutable state and mixing persistence concerns into the public API is fragile.

### 2. SqliteSessionService implementation

**Decision:** A new `SqliteSessionService : ISessionService` in `Peers.Training.Sessions.Internal` that uses `SessionDbContext` for all operations.

**Rationale:** Same namespace and visibility as `InMemorySessionService`. The class receives the `DbContext` via constructor injection and implements the same interface. No lock needed — SQLite serializes writes internally, and EF Core's `SaveChanges` provides the transaction boundary.

Each method maps straightforwardly to EF queries. The token index (`GetParticipantSession`) becomes a query on `Participants.Token` instead of a separate dictionary.

### 3. Registration and configuration

**Decision:** Add `AddSqliteSessions(connectionString)` as a separate extension method alongside the existing `AddSessions()`. The web app chooses which to call based on configuration.

```csharp
// In Program.cs
if (builder.Configuration.GetConnectionString("Sessions") is { } cs)
    builder.Services.AddSqliteSessions(cs);
else
    builder.Services.AddSessions();  // in-memory fallback
```

**Rationale:** Keeps the two implementations independent. No flags or enums — the presence of a connection string is the toggle. In-memory remains the default for local dev (no config needed). On Azure, set the connection string to a file path under `/home`.

**Alternative considered:** A single `AddSessions(options)` method with a provider enum. Rejected — adds abstraction for a two-case switch.

### 4. Migration strategy

**Decision:** Use EF Core migrations. Apply migrations on startup via `context.Database.Migrate()`.

**Rationale:** For a single-instance app with a local SQLite file, auto-migration on startup is safe and simple. No separate migration step in CI/CD. The `Microsoft.EntityFrameworkCore.Design` package is needed as a dev dependency for `dotnet ef migrations add`.

**Alternative considered:** Manual SQL scripts. Rejected — EF Core migrations are low-effort and the schema will grow.

### 5. Test refactoring

**Decision:** Make `SessionServiceSpecificationTests` an abstract class with an abstract `CreateService()` factory. Two concrete subclasses:
- `InMemorySessionServiceTests` — returns `InMemorySessionService` via `AddSessions()`
- `SqliteSessionServiceTests` — returns `SqliteSessionService` with an in-memory SQLite database (`:memory:`)

**Rationale:** Every behavioral test runs against both implementations, ensuring parity. SQLite in-memory mode is fast (no disk I/O) and each test gets a fresh database. The abstract pattern is standard xUnit practice.

The SQLite test class needs to manage the `DbConnection` lifetime — an in-memory SQLite database is destroyed when the connection closes, so the connection must stay open for the duration of each test.

### 6. SQLite file location on Azure

**Decision:** Default connection string points to `Data Source=/home/data/peers.db` on Azure. Locally, `Data Source=peers.db` (current directory).

**Rationale:** On Azure App Service, `/home` is the only directory that persists across restarts. The `/home/data/` subdirectory keeps it organized. Locally, a file in the working directory is the simplest default.

## Risks / Trade-offs

- **SQLite write throughput** — SQLite serializes writes. For a small class tool with low concurrent writes, this is a non-issue. If the app ever needs multi-instance scaling, SQLite would need to be replaced. → Mitigation: The `ISessionService` abstraction makes swapping implementations easy.
- **Auto-migration on startup** — If a migration fails, the app won't start. → Mitigation: EF Core migrations are transactional on SQLite. A failed migration rolls back. Keep migrations small and test them.
- **In-memory SQLite quirks in tests** — The `:memory:` database requires a persistent open connection and doesn't support concurrent access. → Mitigation: Each test creates its own connection and context; no parallelism issues within a single test.
- **No backup strategy** — The SQLite file on Azure has no automated backups. → Mitigation: Acceptable for current scope. The data is ephemeral training sessions, not critical business data.

## Open Questions

- Should `SessionDbContext` be public (for `dotnet ef` CLI access) or internal with an `IDesignTimeDbContextFactory`? Public is simpler; internal is cleaner encapsulation.
