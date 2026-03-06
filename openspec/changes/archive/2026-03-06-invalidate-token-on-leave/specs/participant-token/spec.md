## MODIFIED Requirements

### Requirement: Participant receives a personal token upon joining
The system SHALL issue a unique personal token (UUID) to each participant when they successfully join a session via invite code. The token is scoped to that participant and session. The token SHALL be invalidated when the participant is removed from the session.

#### Scenario: Token issued on successful join
- **WHEN** a participant successfully joins a session via invite code
- **THEN** the system generates a unique UUID as the personal token
- **THEN** the token is stored server-side mapped to the participant's session and dancer name
- **THEN** the token is embedded in the redirect URL to the session view page

#### Scenario: Token is stable across re-joins
- **WHEN** a participant who already has a token re-joins the same session with the same dancer name
- **THEN** the system returns the existing token (does not generate a new one)

#### Scenario: Token is invalidated when participant is removed
- **WHEN** a coach removes a participant from an active session via `LeaveSession`
- **THEN** the system SHALL remove the participant's token from the token index
- **THEN** any subsequent use of that token SHALL be treated as an unknown token
- **THEN** the session view page SHALL display the invalid token error with a link to the join page

#### Scenario: Removed participant re-joins and receives a new token
- **WHEN** a participant who was previously removed joins the same session again via invite code
- **THEN** the system SHALL issue a new token (the old token remains invalid)
- **THEN** the participant is added back to the session roster
