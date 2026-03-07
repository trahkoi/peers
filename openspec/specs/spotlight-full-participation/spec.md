## ADDED Requirements

### Requirement: All participants are paired at least once per round
The pairing algorithm SHALL generate enough pairings so that every dancer in both the leader and follower groups appears in at least one pairing. When groups are unbalanced, dancers from the smaller group SHALL be reused across sub-rounds.

#### Scenario: Balanced groups — all paired in one pass
- **WHEN** there are 3 leaders and 3 followers
- **THEN** exactly 3 pairings SHALL be generated
- **THEN** every leader and every follower SHALL appear exactly once

#### Scenario: More leaders than followers
- **WHEN** there are 5 leaders and 2 followers
- **THEN** at least 5 pairings SHALL be generated (one per leader minimum)
- **THEN** every leader SHALL appear in at least one pairing
- **THEN** followers SHALL be reused across pairings to fill gaps

#### Scenario: More followers than leaders
- **WHEN** there are 2 leaders and 5 followers
- **THEN** at least 5 pairings SHALL be generated (one per follower minimum)
- **THEN** every follower SHALL appear in at least one pairing
- **THEN** leaders SHALL be reused across pairings to fill gaps

#### Scenario: Heavily unbalanced groups
- **WHEN** there are 6 leaders and 1 follower
- **THEN** 6 pairings SHALL be generated
- **THEN** the single follower SHALL appear in all 6 pairings
- **THEN** each leader SHALL appear exactly once

### Requirement: Sub-round pairings minimize historical repeats
When generating additional sub-rounds, the algorithm SHALL treat pairings from earlier sub-rounds as additional history to avoid repeating the same partner across sub-rounds.

#### Scenario: Follower rotates partners across sub-rounds
- **WHEN** there are 4 leaders and 2 followers with no prior history
- **THEN** each follower SHALL be paired with different leaders in each sub-round where possible

### Requirement: Output remains deterministic
The algorithm SHALL produce identical results given identical inputs (leaders, followers, historical counts).

#### Scenario: Repeated calls with same inputs
- **WHEN** `GeneratePairings` is called twice with the same leaders, followers, and history
- **THEN** both calls SHALL return identical pairings in identical order
