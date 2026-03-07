## Why

Spotlight competitions are a core WCS practice format where one couple (leader + follower) dances while others watch. Coaches currently manage pairings manually, which is tedious and leads to uneven distribution — some pairs dance together repeatedly while others rarely get matched. Automating pairing generation with history tracking ensures fair distribution across sessions.

## What Changes

- A new `Peers.Spotlights` project is introduced as an independent module
- A spotlight round can be started for any active session, generating pairings from the session's participants
- The pairing algorithm is deterministic: it minimizes repeat pairings by consulting the full cross-session history
- Pairing history is persisted and spans all sessions, enabling fair distribution over time
- Spotlight pages are added to the web app for managing rounds and viewing pairings

## Capabilities

### New Capabilities

- `spotlight-round`: Start a spotlight round for a session, generating an ordered list of leader/follower pairings
- `pairing-algorithm`: Deterministic algorithm that distributes pairings evenly based on historical pairing counts across all sessions
- `pairing-history`: Persistent record of all spotlight pairings across sessions, queryable per dancer

### Modified Capabilities

None — this is a new module. Depends on `modular-persistence` change being completed first.

## Impact

- New `Peers.Spotlights` project with entities, services, and `IModuleEntityConfiguration` implementation
- New `Peers.Spotlights.Tests` test project
- `Peers.Web` — new Spotlight Razor pages, updated DI registration (`AddSpotlights()`)
- No changes to `Peers.Training` code (reads session participants via `ISessionService`)
- No changes to `Peers.Persistence` code (Spotlights registers its own entity configuration)
- New migration in `Peers.Persistence` for Spotlight tables

## Prerequisites

- `modular-persistence` change must be completed first (shared `PeersDbContext`, `Dancer` entity, `IModuleEntityConfiguration`)
