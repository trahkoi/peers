## MODIFIED Requirements

### Requirement: Admin can generate an invite code for a session
A session admin or coach SHALL be able to generate a short, human-readable invite code for an active session. The admin MUST be authenticated to generate or regenerate a code. The code is unique across all currently active sessions. The same session SHALL only have one invite code at a time; generating a new code replaces the previous one.

#### Scenario: Generate invite code for active session
- **WHEN** an authenticated admin requests an invite code for an active session
- **THEN** the system generates a 6-character uppercase alphanumeric code (excluding ambiguous characters 0, O, 1, I)
- **THEN** the code is stored and associated with the session
- **THEN** the code is displayed on the session manage page

#### Scenario: Generate invite code for ended session
- **WHEN** an authenticated admin requests an invite code for a session that has ended
- **THEN** the system returns an error indicating the session is no longer active

#### Scenario: Regenerate invite code replaces existing code
- **WHEN** an authenticated admin generates an invite code for a session that already has one
- **THEN** the previous code is invalidated
- **THEN** a new code is generated and stored

#### Scenario: Invite code is globally unique
- **WHEN** a new invite code is generated
- **THEN** the system SHALL ensure the code does not collide with any existing active invite code
- **THEN** if a collision occurs the system retries until a unique code is found

#### Scenario: Unauthenticated request to generate invite code is rejected
- **WHEN** an unauthenticated request is made to the generate-code endpoint
- **THEN** the system redirects to `/login` rather than generating a code
