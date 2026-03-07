## Context

The app is an ASP.NET Core Razor Pages app with one domain module (`Peers.Training`) that owns a `TrainingDbContext`. A second module (`Peers.Spotlights`) needs to share the same SQLite database. The architecture must support multiple independent modules contributing entities to a single database without coupling modules to each other.

## Goals / Non-Goals

**Goals:**
- Shared persistence layer that multiple modules can contribute to
- Modules remain independent — no project references between domain modules
- Single migration history in one place
- Add dancer identity to enable cross-session tracking
- Minimal refactoring of existing Training code

**Non-Goals:**
- Separate databases per module
- Distributed transactions or eventual consistency
- Dancer profiles, authentication, or rich identity (just Id + Name)
- Automated dancer matching by name (explicit resolution at join time, for now)

## Decisions

### 1. Shared DbContext with module-contributed configurations

**Decision:** Introduce `PeersDbContext` in a new `Peers.Persistence` project. Modules register `IModuleEntityConfiguration` implementations via DI. The DbContext applies all registered configurations in `OnModelCreating`.

**Rationale:** Modules own their entity definitions but share one DbContext, giving FK integrity across module boundaries without coupling module code. The composition root (Peers.Web) assembles everything.

**Alternative considered:** One DbContext per module pointing at the same SQLite file — simpler isolation but no cross-module FK integrity, and migration ordering becomes manual.

### 2. IModuleEntityConfiguration interface

**Decision:** A simple interface in `Peers.Persistence`:

```csharp
public interface IModuleEntityConfiguration
{
    void Apply(ModelBuilder modelBuilder);
}
```

Each module implements this to configure its entities. `PeersDbContext` resolves all implementations from DI.

**Rationale:** Simpler than scanning assemblies for `IEntityTypeConfiguration<T>`. Explicit, debuggable, no reflection magic. Each module gets one entry point.

**Alternative considered:** Assembly scanning with `modelBuilder.ApplyConfigurationsFromAssembly()` — more convention-based but harder to control ordering and debug.

### 3. Dancer entity lives in Peers.Training

**Decision:** `Dancer` (Id, Name) is defined in `Peers.Training` under a `Dancers/` folder.

**Rationale:** Dancer identity is fundamentally about "who comes to training sessions." Other modules (Spotlights) reference DancerId as an opaque Guid. If a future module needs dancer info, it calls Training's public API.

**Alternative considered:** Separate `Peers.Dancers` project — over-engineering for a Guid + Name record serving two modules.

### 4. Participant links to Dancer

**Decision:** `ParticipantEntity` gains a `DancerId` (Guid) foreign key. On join, the system resolves an existing Dancer by name or creates a new one.

**Rationale:** Simplest approach for name-based matching. Good enough for a practice group where names are known and collisions are rare.

**Risk:** Name collisions (two different people named "Maria"). Acceptable for now — the coach can manage this manually. Future: add disambiguation if needed.

### 5. Migration of existing data

**Decision:** A migration adds the Dancer table and backfills DancerId on existing Participants by creating a Dancer record per unique DancerName.

**Rationale:** Preserves existing data. Idempotent — running the migration twice is safe.

## Dependency Graph

```
                    Peers.Web
                   /         \
                  v           v
        Peers.Training    Peers.Spotlights (future)
                  \           /
                   v         v
               Peers.Persistence
```

Peers.Training and Peers.Spotlights both depend on Peers.Persistence but never on each other.

## Risks / Trade-offs

- **Single migration history** — All modules' schema changes land in `Peers.Persistence/Migrations/`. Conceptual mismatch (entity lives in Training, migration lives in Persistence), but practically fine for EF Core.
- **Name-based dancer matching** — Simple but imprecise. Acceptable for a small practice group.
- **Shared DbContext in DI** — All modules can technically resolve `PeersDbContext` and access any table. Discipline required to only access own entities. Mitigated by keeping entity classes `internal` to each module.

## Migration Plan

1. Create `Peers.Persistence` project with `PeersDbContext` and `IModuleEntityConfiguration`
2. Add `Dancer` entity and `Dancers/` folder to `Peers.Training`
3. Implement `IModuleEntityConfiguration` in `Peers.Training` for all existing entities + Dancer
4. Update `ParticipantEntity` with `DancerId` FK
5. Remove `TrainingDbContext` and update `SqliteSessionService` to use `PeersDbContext`
6. Move and regenerate migrations in `Peers.Persistence`
7. Update `Peers.Web/Program.cs` DI wiring
8. Update tests
