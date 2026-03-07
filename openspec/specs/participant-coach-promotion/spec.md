### Requirement: Coach can promote a participant to session coach
The system SHALL allow an authenticated coach to promote a participant in an active session to a session coach, provided the participant has a personal token (i.e., joined via invite code). Promotion grants the participant management capabilities for that session only. A participant's dance role (Leader/Follower) SHALL remain unchanged by promotion. Participants added manually by the coach (without a token) SHALL NOT be eligible for promotion.

#### Scenario: Successful promotion
- **WHEN** a coach promotes a participant who has a token in an active session
- **THEN** the participant's `IsCoach` flag is set to true
- **THEN** the Manage page reflects the promotion immediately
- **THEN** the participant's dance role (Leader/Follower) is unchanged

#### Scenario: Promotion of participant without token
- **WHEN** a coach attempts to promote a participant who was added manually (no token)
- **THEN** the system returns a validation error indicating only invite-joined participants can be promoted
- **THEN** the Promote button is not shown for tokenless participants in the UI

#### Scenario: Promotion in ended session
- **WHEN** a coach attempts to promote a participant in an ended session
- **THEN** the system returns a validation error: "Cannot modify an ended session"

#### Scenario: Promote an already-promoted participant
- **WHEN** a coach promotes a participant who is already a session coach
- **THEN** the operation succeeds silently (idempotent, no error)

### Requirement: Coach can demote a promoted participant
The system SHALL allow an authenticated coach to demote a promoted participant back to a regular participant. Demotion revokes management capabilities immediately.

#### Scenario: Successful demotion
- **WHEN** a coach demotes a promoted participant
- **THEN** the participant's `IsCoach` flag is set to false
- **THEN** the participant loses access to the Manage page via token
- **THEN** the participant retains their token and can still access the View page

#### Scenario: Demote a non-promoted participant
- **WHEN** a coach demotes a participant who is not promoted
- **THEN** the operation succeeds silently (idempotent, no error)

### Requirement: Promoted participant can manage attendance
A promoted participant SHALL be able to add and remove dancers from the session they are promoted in, using the Manage page accessed via their participant token.

#### Scenario: Promoted participant adds a dancer
- **WHEN** a promoted participant accesses the Manage page via token and adds a dancer
- **THEN** the dancer is added to the session

#### Scenario: Promoted participant removes a dancer
- **WHEN** a promoted participant accesses the Manage page via token and removes a dancer
- **THEN** the dancer is removed from the session

#### Scenario: Non-promoted participant cannot access Manage page
- **WHEN** a regular (non-promoted) participant navigates to the Manage page with their token
- **THEN** the system denies access and redirects to the View page

### Requirement: Promoted participant can generate spotlight pairings
A promoted participant SHALL be able to generate spotlight pairings for the session they are promoted in.

#### Scenario: Promoted participant generates spotlight round
- **WHEN** a promoted participant accesses the Manage page via token and triggers spotlight generation
- **THEN** a new spotlight round is generated for the session

### Requirement: Promoted participant cannot perform restricted actions
A promoted participant SHALL NOT be able to end the session, generate invite codes, or promote/demote other participants. These actions remain exclusive to authenticated coaches.

#### Scenario: Promoted participant attempts to end session
- **WHEN** a promoted participant attempts to end the session via the Manage page
- **THEN** the action is not available (button/form not rendered)

#### Scenario: Promoted participant attempts to generate invite code
- **WHEN** a promoted participant attempts to generate an invite code
- **THEN** the action is not available (button/form not rendered)

#### Scenario: Promoted participant attempts to promote another participant
- **WHEN** a promoted participant attempts to promote or demote another participant
- **THEN** the action is not available (button/form not rendered)

### Requirement: Promotion is invalidated when participant is removed
When a promoted participant is removed from the session, their promotion SHALL be revoked along with their token.

#### Scenario: Removed promoted participant loses all access
- **WHEN** a coach removes a promoted participant from the session
- **THEN** the participant's token is invalidated
- **THEN** the participant can no longer access the Manage page or View page
