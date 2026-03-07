## Why

Currently only authenticated admin users (coaches) can manage attendance and generate spotlight pairings via the Manage page. In practice, a coach often wants to hand off session management to a trusted dancer — for example when they're dancing themselves or running multiple sessions. There's no way to do this without sharing the admin login.

## What Changes

- A coach can promote any participant in an active session to a "coach" role, granting them access to the Manage page for that session
- Promoted participants can manage attendance (add/remove dancers) and generate spotlight pairings, scoped to their session only
- Promoted participants access the Manage page via their existing participant token — no login required
- A coach can demote a promoted participant back to a regular dancer
- Promotion is session-scoped and does not persist beyond the session

## Capabilities

### New Capabilities
- `participant-coach-promotion`: Rules for promoting a participant to coach within a session, including promotion/demotion actions and the permissions granted

### Modified Capabilities
- `participant-token`: Promoted participants need their token to grant Manage page access instead of just read-only View access

## Impact

- **Peers.Training**: `Participant` record needs a coach flag or new role concept. `ISessionService` needs promote/demote methods
- **Peers.Web**: Manage page authorization must allow token-based access for promoted participants. View page may show a link to Manage for promoted participants
- **Peers.Persistence**: Participant entity needs a new column for coach/promoted status; migration required
