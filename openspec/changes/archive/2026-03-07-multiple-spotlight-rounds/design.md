## Context

The Spotlights module currently enforces a 1:1 relationship between sessions and spotlight rounds. `SpotlightService.GenerateRound` deletes any existing round before inserting a new one, and `GetRound` returns a single nullable round. The Manage page displays one round and offers a "Generate Spotlight" button.

Coaches typically run 3-5 spotlight rounds per session. Supporting multiple rounds requires changing the data model, service API, and page to work with a collection of rounds.

## Goals / Non-Goals

**Goals:**
- A session can accumulate multiple spotlight rounds, each with its own pairings
- Each round has a sequential number within its session (Round 1, Round 2, ...)
- The pairing algorithm's historical counts span all rounds in the session, continuing to minimize repeat pairings
- The Manage page shows all rounds and supports generating new ones

**Non-Goals:**
- Deleting or reordering individual rounds
- Editing pairings within a round after generation
- Limiting the maximum number of rounds per session

## Decisions

### 1. Add `RoundNumber` column to `SpotlightRoundEntity`

Computed at insert time as `MAX(RoundNumber) + 1` for the session. Simpler than deriving from ordering or timestamps. This gives stable, human-readable labels ("Round 1", "Round 2").

**Alternative**: Use `CreatedAt` ordering and derive display numbers. Rejected because it couples display to timestamp precision and makes queries less intuitive.

### 2. Replace `GetRound` with `GetRounds` returning all rounds

The service returns `IReadOnlyList<SpotlightRound>` ordered by `RoundNumber`. The page model binds to the full list.

A `GetLatestRound` convenience method is not needed — the page can pick the last item from the list. Keep the API surface small.

### 3. Remove delete-on-generate from `GenerateRound`

The current code deletes the existing round before inserting. This line is simply removed — `GenerateRound` only inserts.

### 4. Migration adds `RoundNumber` with default value

Existing data (at most one round per session) gets `RoundNumber = 1` via a default value in the migration. No data migration script needed.

## Risks / Trade-offs

- **Unbounded rounds**: No limit on rounds per session means a coach could generate many rounds. Low risk — this is a small-scale app and the pairing algorithm is fast. If needed, a limit can be added later.
- **Breaking API change**: `GetRound` → `GetRounds` is a compile-time breaking change within the solution. Acceptable since there are no external consumers; the only caller is `ManageModel`.
