### Requirement: A session supports multiple spotlight rounds
The system SHALL allow generating multiple spotlight rounds for a single session. Each call to generate a round SHALL append a new round without removing previous rounds.

#### Scenario: Generating a second round preserves the first
- **WHEN** a spotlight round has already been generated for a session
- **AND** the coach generates another spotlight round
- **THEN** both rounds SHALL be stored and retrievable for that session

#### Scenario: Generating rounds across sessions are independent
- **WHEN** rounds are generated for session A and session B
- **THEN** each session's rounds SHALL be independent and numbered separately

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

### Requirement: All rounds for a session are retrievable
The system SHALL provide a way to retrieve all spotlight rounds for a given session, ordered by round number ascending.

#### Scenario: Retrieving rounds returns them in order
- **WHEN** a session has 3 spotlight rounds
- **THEN** retrieving rounds SHALL return all 3 ordered by round number (1, 2, 3)

#### Scenario: No rounds exist
- **WHEN** no spotlight rounds have been generated for a session
- **THEN** retrieving rounds SHALL return an empty list

### Requirement: Historical pairing counts span all rounds
When generating a new round, the pairing algorithm SHALL consider pairings from all previous rounds in the session (and any other sessions involving the same dancers) as historical data to minimize repeat pairings.

#### Scenario: Third round avoids pairings from rounds 1 and 2
- **WHEN** a session has two existing rounds with pairings
- **AND** a third round is generated
- **THEN** the algorithm SHALL factor in pairings from both prior rounds when minimizing repeats
