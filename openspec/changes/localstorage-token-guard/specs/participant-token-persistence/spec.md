## ADDED Requirements

### Requirement: Token is persisted to localStorage after successful join
After a participant successfully joins a session and is shown the session view, the browser SHALL store the participant's token and session name in `localStorage` under the key `peers_participant_token`.

#### Scenario: Token written on view page load with valid token
- **WHEN** a participant loads `/sessions/view?token=<valid-token>`
- **THEN** the browser writes `{ token, sessionName }` to `localStorage["peers_participant_token"]`

#### Scenario: localStorage write failure is silent
- **WHEN** `localStorage` is unavailable (e.g. private browsing with strict settings)
- **THEN** the page loads normally without error

### Requirement: Join page shows a notice when a stored token is found
When a participant navigates to the join page and the browser has a stored token in `localStorage`, the page SHALL display a non-blocking notice informing them they are already in a session, with a link to return to their session view.

#### Scenario: Stored token found on join page
- **WHEN** a participant navigates to `/sessions/join`
- **AND** `localStorage["peers_participant_token"]` contains a token and session name
- **THEN** a notice is shown: "You're already in [sessionName] — view it here"
- **THEN** the notice contains a link to `/sessions/view?token=<stored-token>`
- **THEN** the join form is still visible and usable

#### Scenario: No stored token on join page
- **WHEN** a participant navigates to `/sessions/join`
- **AND** `localStorage["peers_participant_token"]` is absent or empty
- **THEN** no notice is shown and the join form is displayed normally

#### Scenario: Joining a new session overwrites stored token
- **WHEN** a participant submits the join form successfully while a token is already stored
- **THEN** the new token overwrites the previous entry in `localStorage`

### Requirement: localStorage is cleared when stored token is invalid
If a participant visits the session view page with a stored token that the server does not recognise (e.g. after a server restart), the browser SHALL clear `localStorage` so the participant can re-join without being stuck in a loop.

#### Scenario: Token not found clears localStorage
- **WHEN** a participant loads `/sessions/view?token=<token>` and the server returns "token not found"
- **THEN** `localStorage["peers_participant_token"]` is removed
- **THEN** the "Session not found" error state is shown with a link to the join page
