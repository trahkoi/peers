## Why

Currently each session supports only one spotlight round — generating a new round replaces the previous one. Coaches need to run multiple spotlight rounds during a single session so dancers get more practice with different partners.

## What Changes

- Remove the 1:1 constraint between sessions and spotlight rounds — a session can have many rounds
- `GenerateRound` appends a new round instead of replacing the existing one
- `GetRound` is replaced with `GetRounds` (returns all rounds for a session) and optionally `GetLatestRound`
- The Manage page displays all rounds (or at minimum the latest) and allows generating additional ones
- Round number is tracked to give each round a sequential label within its session
- Historical pairing counts span all rounds in the session, so the algorithm continues to minimize repeat pairings across rounds

## Capabilities

### New Capabilities

- `multiple-rounds-per-session`: Support creating and retrieving multiple spotlight rounds within a single training session

### Modified Capabilities

_None_ — the full-participation and pairing algorithm requirements remain unchanged; they already operate per-round.

## Impact

- **Peers.Spotlights**: `ISpotlightService` API changes (`GetRound` → `GetRounds`/`GetLatestRound`), `SpotlightService` removes delete-on-generate logic, `SpotlightRound` gains a `RoundNumber` property, `SpotlightRoundEntity` gains a `RoundNumber` column
- **Peers.Web**: `ManageModel` updates to handle a list of rounds and display round numbers
- **Peers.Persistence**: New EF Core migration for the `RoundNumber` column
- **Tests**: `SpotlightServiceTests` updated for multi-round scenarios
