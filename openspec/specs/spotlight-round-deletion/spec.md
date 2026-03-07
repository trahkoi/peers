### Requirement: A coach can delete a spotlight round
The system SHALL allow deleting a spotlight round by its ID. Deleting a round SHALL remove the round and all its associated pairings from the database.

#### Scenario: Deleting an existing round removes it and its pairings
- **WHEN** a spotlight round with pairings exists for a session
- **AND** the round is deleted by its ID
- **THEN** the round SHALL no longer appear in the session's rounds list
- **AND** all pairings associated with that round SHALL be removed

#### Scenario: Deleting a non-existent round has no effect
- **WHEN** a delete is requested for a round ID that does not exist
- **THEN** the system SHALL complete without error

### Requirement: Deleted round pairings do not influence future rounds
When a spotlight round is deleted, its pairings SHALL no longer be considered as historical data by the pairing algorithm when generating subsequent rounds.

#### Scenario: Generating a round after deletion ignores deleted pairings
- **WHEN** a round is generated, then deleted
- **AND** a new round is generated for the same session with the same participants
- **THEN** the pairing algorithm SHALL NOT consider the deleted round's pairings as historical data
