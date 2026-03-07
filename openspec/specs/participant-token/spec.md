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

### Requirement: Participant can re-enter the session using their personal token
A participant SHALL be able to navigate to their session view by visiting a URL that contains their personal token. This allows recovery after closing the browser tab or losing the session. If the participant is promoted to session coach, the token SHALL also grant access to the Manage page.

#### Scenario: Valid token grants session view access
- **WHEN** a participant navigates to `/sessions/view?token={token}` with a valid token
- **THEN** the system looks up the session and dancer name associated with the token
- **THEN** the participant is shown the read-only session view for their session

#### Scenario: Valid token for promoted participant shows manage link
- **WHEN** a promoted participant navigates to `/sessions/view?token={token}` with a valid token
- **THEN** the session view page includes a link to the Manage page with the token embedded

#### Scenario: Invalid or unknown token
- **WHEN** a participant navigates to `/sessions/view?token={token}` with an unknown token
- **THEN** the system displays an error: "Session not found. Your link may have expired."
- **THEN** the participant is offered a link to the join page

#### Scenario: Token invalidated when session ends
- **WHEN** a session ends and a participant tries to use their token
- **THEN** the system shows the session view in a read-only ended state (not an error)
- **THEN** the session roster is still visible but no changes can be made

#### Scenario: Promoted participant accesses Manage page via token
- **WHEN** a promoted participant navigates to `/sessions/manage?token={token}`
- **THEN** the system grants access to the Manage page for the associated session
- **THEN** restricted actions (end session, invite codes, promote/demote) are hidden

#### Scenario: Non-promoted participant denied Manage page access via token
- **WHEN** a regular participant navigates to `/sessions/manage?token={token}`
- **THEN** the system redirects to `/sessions/view?token={token}`

### Requirement: Session view page shows session state to participants
The session view page (`/sessions/view`) SHALL display the session name, current status (active/ended), and the list of participants with their roles. It is read-only and requires no login — only a valid personal token.

#### Scenario: Active session view
- **WHEN** a participant opens the session view with a valid token for an active session
- **THEN** the page shows the session name, status "Active", and the participant roster

#### Scenario: Ended session view
- **WHEN** a participant opens the session view with a valid token for an ended session
- **THEN** the page shows the session name, status "Ended", and the participant roster
- **THEN** no join or leave actions are available
