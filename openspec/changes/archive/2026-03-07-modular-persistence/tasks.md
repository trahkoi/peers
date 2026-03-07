## 1. Create Peers.Persistence project

- [x] 1.1 Create `src/Peers.Persistence/Peers.Persistence.csproj` with EF Core SQLite and DI abstractions references
- [x] 1.2 Define `IModuleEntityConfiguration` interface with `void Apply(ModelBuilder modelBuilder)`
- [x] 1.3 Define `PeersDbContext` that resolves `IEnumerable<IModuleEntityConfiguration>` from DI and applies them in `OnModelCreating`
- [x] 1.4 ~~Add `PeersDbContextFactory` for EF Core design-time tooling~~ Not needed — EF Core uses the startup project's DI
- [x] 1.5 Add DI extension method `AddPeersDb(connectionString)` that registers `PeersDbContext`

## 2. Add Dancer entity to Peers.Training

- [x] 2.1 Create `Dancers/DancerEntity.cs` (internal) with `Id` (Guid) and `Name` (string)
- [x] 2.2 Create `Dancers/Dancer.cs` (public DTO/record) for the module's public API
- [x] 2.3 Add `DancerId` (Guid) property to `ParticipantEntity`

## 3. Refactor Peers.Training to use shared persistence

- [x] 3.1 Add project reference from `Peers.Training` to `Peers.Persistence`
- [x] 3.2 Implement `IModuleEntityConfiguration` in `Peers.Training` configuring `SessionEntity`, `ParticipantEntity` (with DancerId FK), and `DancerEntity`
- [x] 3.3 Update `SqliteSessionService` to use `PeersDbContext` instead of `TrainingDbContext`
- [x] 3.4 Update join logic to resolve or create a `Dancer` by name and link `Participant.DancerId`
- [x] 3.5 Remove `TrainingDbContext` and `TrainingDbContextFactory`
- [x] 3.6 Update `SessionServiceCollectionExtensions` — replace `AddSqliteSessions` with `AddTraining()` that registers the module configuration and services

## 4. Migrations

- [x] 4.1 Delete old migrations from `Peers.Training/Persistence/Migrations/`
- [x] 4.2 Generate fresh initial migration in `Peers.Persistence` covering all Training entities including Dancer
- [x] 4.3 Add `MigratePeersDatabase()` extension method to `Peers.Persistence`

## 5. Update Peers.Web

- [x] 5.1 Add project reference from `Peers.Web` to `Peers.Persistence`
- [x] 5.2 Update `Program.cs` to call `AddPeersDb(connectionString)` and `AddTraining()` instead of `AddSqliteSessions(connectionString)`
- [x] 5.3 Update `Program.cs` to call `MigratePeersDatabase()` instead of `MigrateSessionsDatabase()`

## 6. Update tests

- [x] 6.1 Update `Peers.Training.Tests` to reference `Peers.Persistence` and use `PeersDbContext`
- [x] 6.2 Verify existing session tests pass with the new DbContext
