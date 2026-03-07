## MODIFIED Requirements

### Requirement: Each round has a sequential round number
Each spotlight round SHALL have a `RoundNumber` that is assigned sequentially within its session, starting at 1 and incrementing by 1 for each new round. After a round is deleted, existing round numbers SHALL NOT be renumbered; gaps in the sequence are acceptable.

#### Scenario: First round gets number 1
- **WHEN** the first spotlight round is generated for a session
- **THEN** it SHALL have `RoundNumber` equal to 1

#### Scenario: Subsequent rounds increment
- **WHEN** a session already has rounds numbered 1 through N
- **AND** a new round is generated
- **THEN** the new round SHALL have `RoundNumber` equal to N + 1

#### Scenario: Deleting a round does not renumber others
- **WHEN** a session has rounds numbered 1, 2, 3
- **AND** round 2 is deleted
- **THEN** the remaining rounds SHALL have numbers 1 and 3
