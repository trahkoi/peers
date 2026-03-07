## Why

The app currently has one module (`Peers.Training`) with its own `TrainingDbContext`. A second module (`Peers.Spotlights`) is coming, and both need to persist to the same SQLite database. The current structure doesn't support multiple modules contributing entities to a shared database without coupling them together.

Additionally, there is no persistent dancer identity — participants are session-scoped. Cross-session features (like spotlight pairing history) require a stable dancer identity that survives across sessions.

## What Changes

- A new `Peers.Persistence` project is introduced with a shared `PeersDbContext` and an `IModuleEntityConfiguration` interface
- Each module contributes its entity configurations via DI — modules don't reference each other
- `Peers.Training` is refactored to use the shared context instead of owning `TrainingDbContext`
- A `Dancer` entity is added to `Peers.Training`, and `Participant` is linked to a `DancerId`
- Migrations move from `Peers.Training` to `Peers.Persistence`

## Capabilities

### New Capabilities

- `shared-persistence`: A shared `PeersDbContext` that assembles entity configurations from all registered modules at startup, with a single migration history
- `dancer-identity`: A lightweight dancer identity (`Id`, `Name`) in the Training module, linked from `Participant`, enabling cross-session tracking

### Modified Capabilities

- `session-join`: Joining a session now resolves or creates a `Dancer` record and links the `Participant` to it

## Impact

- New `Peers.Persistence` project (shared DbContext, interface, migrations)
- `Peers.Training` — remove `TrainingDbContext`, add `Dancer` entity, implement `IModuleEntityConfiguration`, update `SqliteSessionService` to use `PeersDbContext`
- `Peers.Web` — update `Program.cs` DI wiring to register shared DbContext and module configurations
- `Peers.Training.Tests` — update to use `PeersDbContext`
- Existing data requires a migration (add Dancer table, add DancerId FK to Participants)
