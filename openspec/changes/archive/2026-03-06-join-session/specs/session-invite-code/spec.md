## ADDED Requirements

### Requirement: Admin can generate an invite code for a session
A session admin or coach SHALL be able to generate a short, human-readable invite code for an active session. The code is unique across all currently active sessions. The same session SHALL only have one invite code at a time; generating a new code replaces the previous one.

#### Scenario: Generate invite code for active session
- **WHEN** an admin requests an invite code for an active session
- **THEN** the system generates a 6-character uppercase alphanumeric code (excluding ambiguous characters 0, O, 1, I)
- **THEN** the code is stored and associated with the session
- **THEN** the code is displayed on the session manage page

#### Scenario: Generate invite code for ended session
- **WHEN** an admin requests an invite code for a session that has ended
- **THEN** the system returns an error indicating the session is no longer active

#### Scenario: Regenerate invite code replaces existing code
- **WHEN** an admin generates an invite code for a session that already has one
- **THEN** the previous code is invalidated
- **THEN** a new code is generated and stored

#### Scenario: Invite code is globally unique
- **WHEN** a new invite code is generated
- **THEN** the system SHALL ensure the code does not collide with any existing active invite code
- **THEN** if a collision occurs the system retries until a unique code is found

### Requirement: Invite code is displayed on the manage page
The session manage page SHALL display the current invite code for the session, if one has been generated. Admins SHALL be able to copy or share it from that page.

#### Scenario: No invite code yet
- **WHEN** an admin views the manage page for a session with no invite code
- **THEN** the page shows a "Generate invite code" button and no code is displayed

#### Scenario: Invite code exists
- **WHEN** an admin views the manage page for a session that has an invite code
- **THEN** the page displays the invite code prominently
- **THEN** the page shows a "Regenerate" button to replace the code
