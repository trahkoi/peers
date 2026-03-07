## MODIFIED Requirements

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
