## 1. EF Core Setup

- [x] 1.1 Add `Microsoft.EntityFrameworkCore.Sqlite` and `Microsoft.EntityFrameworkCore.Design` NuGet packages to `Peers.Training`
- [x] 1.2 Create `SessionEntity` and `ParticipantEntity` classes in `Peers.Training.Sessions.Internal`
- [x] 1.3 Create `SessionDbContext` with entity configurations (unique indexes on Token, InviteCode, and SessionId+DancerName composite; cascade delete on Session→Participants)
- [x] 1.4 Generate the initial EF Core migration

## 2. SqliteSessionService Implementation

- [x] 2.1 Create `SqliteSessionService : ISessionService` in `Peers.Training.Sessions.Internal` implementing all interface methods against `SessionDbContext`
- [x] 2.2 Add `AddSqliteSessions(connectionString)` extension method in `SessionServiceCollectionExtensions` that registers the DbContext and SqliteSessionService

## 3. Web Integration

- [x] 3.1 Update `Program.cs` to select SQLite or in-memory based on `ConnectionStrings:Sessions` configuration
- [x] 3.2 Add auto-migration on startup (`context.Database.Migrate()`) when SQLite is configured
- [x] 3.3 Add a default connection string in `appsettings.Development.json` for local dev (e.g. `Data Source=peers.db`)

## 4. Test Refactoring

- [x] 4.1 Add `Microsoft.EntityFrameworkCore.Sqlite` NuGet package to `Peers.Training.Tests`
- [x] 4.2 Refactor `SessionServiceSpecificationTests` into an abstract base class with abstract `CreateService()` and optional `DisposeService()` methods
- [x] 4.3 Create `InMemorySessionServiceTests` subclass that returns `InMemorySessionService`
- [x] 4.4 Create `SqliteSessionServiceTests` subclass that returns `SqliteSessionService` with an in-memory SQLite database (open connection kept alive per test)
- [x] 4.5 Verify all tests pass for both implementations

## 5. Azure Configuration

- [x] 5.1 Add `ConnectionStrings:Sessions` app setting to Bicep template pointing to `/home/data/peers.db`
- [ ] 5.2 Verify deployment creates the database file and sessions survive a restart (manual verification needed)
