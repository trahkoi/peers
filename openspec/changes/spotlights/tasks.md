## Prerequisites

- `modular-persistence` change must be completed first

## 1. Create Peers.Spotlights project

- [ ] 1.1 Create `src/Peers.Spotlights/Peers.Spotlights.csproj` with references to `Peers.Persistence` and `Peers.Training`
- [ ] 1.2 Create `tests/Peers.Spotlights.Tests/Peers.Spotlights.Tests.csproj`

## 2. Domain entities

- [ ] 2.1 Create `SpotlightRoundEntity` (internal) with Id, SessionId, CreatedAt, Pairings collection
- [ ] 2.2 Create `PairingEntity` (internal) with Id, RoundId, LeaderDancerId, FollowerDancerId, Order
- [ ] 2.3 Create public DTOs/records: `SpotlightRound`, `Pairing`

## 3. Entity configuration and migration

- [ ] 3.1 Implement `IModuleEntityConfiguration` in `SpotlightsEntityConfiguration` — configure both entities, unique index on SessionId
- [ ] 3.2 Generate migration in `Peers.Persistence` for SpotlightRound and Pairing tables

## 4. Pairing algorithm

- [ ] 4.1 Implement `PairingAlgorithm` — input: list of (DancerId, Role) + historical pairing counts; output: ordered list of (LeaderDancerId, FollowerDancerId)
- [ ] 4.2 Handle unbalanced roles (more leaders than followers or vice versa) — dancers with highest recent activity sit out
- [ ] 4.3 Unit tests for the algorithm: balanced groups, unbalanced groups, no history, heavy history, same-person exclusion

## 5. Spotlight service

- [ ] 5.1 Define `ISpotlightService` interface: `GenerateRound(sessionId)`, `GetRound(sessionId)`, `GetDancerHistory(dancerId)`
- [ ] 5.2 Implement `SpotlightService` — calls `ISessionService.GetParticipants()`, queries pairing history, runs algorithm, persists round
- [ ] 5.3 Add DI extension method `AddSpotlights()` registering entity configuration and service
- [ ] 5.4 Integration tests with in-memory SQLite

## 6. Training module: expose participants with DancerId

- [ ] 6.1 Ensure `ISessionService` has a method that returns participants with their DancerId and Role (add if missing)

## 7. Web integration

- [ ] 7.1 Update `Peers.Web.csproj` to reference `Peers.Spotlights`
- [ ] 7.2 Register `AddSpotlights()` in `Program.cs`
- [ ] 7.3 Create Spotlight page on the session manage view — button to generate/regenerate pairings
- [ ] 7.4 Display pairing list with order, leader name, follower name
- [ ] 7.5 Protect Spotlight pages with `[Authorize]` (admin/coach only)
