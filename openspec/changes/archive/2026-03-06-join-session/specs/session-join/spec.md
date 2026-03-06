## ADDED Requirements

### Requirement: Participant can join a session via invite code
A participant SHALL be able to join a training session by entering a valid invite code, their dancer name, and their role (Leader or Follower) on a public join page. No login is required.

#### Scenario: Successful join with valid invite code
- **WHEN** a participant submits a valid invite code, a non-empty dancer name, and a valid role
- **THEN** the participant is added to the session
- **THEN** the system issues a personal participant token
- **THEN** the participant is redirected to the session view page with the token embedded in the URL

#### Scenario: Join with invalid or unknown invite code
- **WHEN** a participant submits an invite code that does not match any active session
- **THEN** the system returns a validation error: "Invite code not found or session has ended"
- **THEN** the join form is re-displayed with the error

#### Scenario: Join a session that has ended
- **WHEN** a participant submits an invite code for a session that has ended
- **THEN** the system returns an error: "Invite code not found or session has ended"

#### Scenario: Join with missing dancer name
- **WHEN** a participant submits a valid invite code but an empty dancer name
- **THEN** the system returns a validation error: "Dancer name is required"
- **THEN** the join form is re-displayed with the error

#### Scenario: Re-join with same name returns existing token
- **WHEN** a participant submits a valid invite code and a dancer name that is already in the session
- **THEN** the system does NOT add a duplicate entry
- **THEN** the system returns the existing personal token for that participant
- **THEN** the participant is redirected to the session view page

### Requirement: Join page is publicly accessible
The join page (`/sessions/join`) SHALL be accessible without authentication. It accepts an optional `code` query parameter to pre-fill the invite code field.

#### Scenario: Join page pre-filled via query parameter
- **WHEN** a user navigates to `/sessions/join?code=A3F9KX`
- **THEN** the invite code field is pre-populated with `A3F9KX`
- **THEN** the user only needs to enter their name and role

#### Scenario: Join page without query parameter
- **WHEN** a user navigates to `/sessions/join` with no query parameter
- **THEN** all fields are empty and the user must fill in the code, name, and role
