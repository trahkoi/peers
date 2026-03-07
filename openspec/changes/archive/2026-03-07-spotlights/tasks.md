## Prerequisites

- `modular-persistence` change must be completed first

## 1. Create Peers.Spotlights project

- [x] 1.1 Create `src/Peers.Spotlights/Peers.Spotlights.csproj` with references to `Peers.Persistence` and `Peers.Training`
- [x] 1.2 Create `tests/Peers.Spotlights.Tests/Peers.Spotlights.Tests.csproj`

## 2. Domain entities

- [x] 2.1 Create `SpotlightRoundEntity` (internal) with Id, SessionId, CreatedAt, Pairings collection
- [x] 2.2 Create `PairingEntity` (internal) with Id, RoundId, LeaderDancerId, FollowerDancerId, Order
- [x] 2.3 Create public DTOs/records: `SpotlightRound`, `Pairing`

## 3. Entity configuration and migration

- [x] 3.1 Implement `IModuleEntityConfiguration` in `SpotlightsEntityConfiguration` — configure both entities, unique index on SessionId
- [x] 3.2 Generate migration in `Peers.Persistence` for SpotlightRound and Pairing tables

## 4. Pairing algorithm

- [x] 4.1 Implement `PairingAlgorithm` — input: list of (DancerId, Role) + historical pairing counts; output: ordered list of (LeaderDancerId, FollowerDancerId)
- [x] 4.2 Handle unbalanced roles (more leaders than followers or vice versa) — dancers with highest recent activity sit out
- [x] 4.3 Unit tests for the algorithm: balanced groups, unbalanced groups, no history, heavy history, same-person exclusion

## 5. Spotlight service

- [x] 5.1 Define `ISpotlightService` interface: `GenerateRound(sessionId)`, `GetRound(sessionId)`
- [x] 5.2 Implement `SpotlightService` — calls `ISessionService.GetParticipants()`, queries pairing history, runs algorithm, persists round
- [x] 5.3 Add DI extension method `AddSpotlights()` registering entity configuration and service
- [x] 5.4 Integration tests with in-memory SQLite

## 6. Training module: expose participants with DancerId

- [x] 6.1 Ensure `ISessionService` has a method that returns participants with their DancerId and Role (add if missing)

## 7. Web integration

- [x] 7.1 Update `Peers.Web.csproj` to reference `Peers.Spotlights`
- [x] 7.2 Register `AddSpotlights()` in `Program.cs`
- [x] 7.3 Create Spotlight section on the session manage view — button to generate/regenerate pairings
- [x] 7.4 Display pairing list with order, leader name, follower name
- [x] 7.5 Spotlight UI is on the Manage page which is already protected with `[Authorize]`
