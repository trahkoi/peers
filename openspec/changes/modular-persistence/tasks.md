## 1. Create Peers.Persistence project

- [ ] 1.1 Create `src/Peers.Persistence/Peers.Persistence.csproj` with EF Core SQLite and DI abstractions references
- [ ] 1.2 Define `IModuleEntityConfiguration` interface with `void Apply(ModelBuilder modelBuilder)`
- [ ] 1.3 Define `PeersDbContext` that resolves `IEnumerable<IModuleEntityConfiguration>` from DI and applies them in `OnModelCreating`
- [ ] 1.4 Add `PeersDbContextFactory` for EF Core design-time tooling
- [ ] 1.5 Add DI extension method `AddPeersDb(connectionString)` that registers `PeersDbContext`

## 2. Add Dancer entity to Peers.Training

- [ ] 2.1 Create `Dancers/DancerEntity.cs` (internal) with `Id` (Guid) and `Name` (string)
- [ ] 2.2 Create `Dancers/Dancer.cs` (public DTO/record) for the module's public API
- [ ] 2.3 Add `DancerId` (Guid) property to `ParticipantEntity`

## 3. Refactor Peers.Training to use shared persistence

- [ ] 3.1 Add project reference from `Peers.Training` to `Peers.Persistence`
- [ ] 3.2 Implement `IModuleEntityConfiguration` in `Peers.Training` configuring `SessionEntity`, `ParticipantEntity` (with DancerId FK), and `DancerEntity`
- [ ] 3.3 Update `SqliteSessionService` to use `PeersDbContext` instead of `TrainingDbContext`
- [ ] 3.4 Update join logic to resolve or create a `Dancer` by name and link `Participant.DancerId`
- [ ] 3.5 Remove `TrainingDbContext` and `TrainingDbContextFactory`
- [ ] 3.6 Update `SessionServiceCollectionExtensions` — replace `AddSqliteSessions` with `AddTraining()` that registers the module configuration and services

## 4. Migrations

- [ ] 4.1 Delete old migrations from `Peers.Training/Persistence/Migrations/`
- [ ] 4.2 Generate fresh initial migration in `Peers.Persistence` covering all Training entities including Dancer
- [ ] 4.3 Add `MigratePeersDatabase()` extension method to `Peers.Persistence`

## 5. Update Peers.Web

- [ ] 5.1 Add project reference from `Peers.Web` to `Peers.Persistence`
- [ ] 5.2 Update `Program.cs` to call `AddPeersDb(connectionString)` and `AddTraining()` instead of `AddSqliteSessions(connectionString)`
- [ ] 5.3 Update `Program.cs` to call `MigratePeersDatabase()` instead of `MigrateSessionsDatabase()`

## 6. Update tests

- [ ] 6.1 Update `Peers.Training.Tests` to reference `Peers.Persistence` and use `PeersDbContext`
- [ ] 6.2 Verify existing session tests pass with the new DbContext
