## Why

Sessions and participants are stored in memory. Every app restart or redeploy wipes all state — active sessions, invite codes, and participant tokens are lost. On Azure App Service, this happens on every deploy and on idle restarts. For the app to be usable in practice, session data must survive process restarts.

## What Changes

- Add a new `SqliteSessionService` implementation of `ISessionService` backed by SQLite via EF Core
- Introduce an EF Core `DbContext` with `Sessions` and `Participants` tables in `Peers.Training`
- Add EF Core migration infrastructure for schema evolution
- Add a registration method to wire up the SQLite implementation via configuration
- Refactor existing tests into an abstract specification base class so both implementations are tested against the same contract
- Keep `InMemorySessionService` for local development and fast unit tests

## Capabilities

### New Capabilities
- `sqlite-session-storage`: Persistent storage of sessions and participants in a SQLite database via EF Core, including schema, DbContext, migrations, and a new `ISessionService` implementation

### Modified Capabilities
_None. All existing behavioral requirements (participant-token, session-invite-code, session-join) remain unchanged. This is a storage layer change — the contract stays the same._

## Impact

- **Peers.Training**: New NuGet dependencies (`Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design`). New `DbContext`, entity configurations, and `SqliteSessionService` class. New or modified `AddSessions` registration.
- **Peers.Web**: Connection string configuration in `appsettings.json` / environment variables. EF Core migration application on startup (or via CLI).
- **Peers.Training.Tests**: New NuGet dependency (`Microsoft.EntityFrameworkCore.Sqlite`). Test refactor: abstract base class + two concrete test classes.
- **Azure deployment**: SQLite file must be placed on `/home` (persistent storage on App Service). No new Azure resources needed.
