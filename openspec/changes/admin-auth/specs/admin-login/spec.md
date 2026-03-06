## ADDED Requirements

### Requirement: Admin can log in with configured credentials
The system SHALL provide a login page at `/login` where an admin or coach enters a username and password. On success, an authentication cookie is issued and the admin is redirected to their intended destination. No registration flow exists — credentials are configured in app settings.

#### Scenario: Successful login with valid credentials
- **WHEN** an admin submits the correct username and password on `/login`
- **THEN** the system issues an authentication cookie
- **THEN** the admin is redirected to the `returnUrl` query parameter if present and local, otherwise to `/sessions`

#### Scenario: Login with invalid credentials
- **WHEN** an admin submits an incorrect username or password
- **THEN** the system redisplays the login form with an error: "Invalid username or password"
- **THEN** no authentication cookie is issued

#### Scenario: Login form preserves returnUrl
- **WHEN** an unauthenticated user is redirected to `/login?returnUrl=/sessions/new`
- **THEN** the login form action includes the `returnUrl` so the redirect target is preserved after successful login

### Requirement: Protected pages require authentication
The session list (`/sessions`), session creation (`/sessions/new`), and session management (`/sessions/{id}`) pages SHALL require an authenticated admin. Unauthenticated requests SHALL be redirected to `/login` with the original path as `returnUrl`.

#### Scenario: Unauthenticated access to protected page
- **WHEN** an unauthenticated user navigates to `/sessions`
- **THEN** the system redirects to `/login?returnUrl=/sessions`

#### Scenario: Authenticated access to protected page
- **WHEN** an authenticated admin navigates to `/sessions`
- **THEN** the page is shown normally

### Requirement: Public pages remain accessible without login
The participant join page (`/sessions/join`) and session view page (`/sessions/view`) SHALL remain accessible without authentication.

#### Scenario: Unauthenticated access to join page
- **WHEN** an unauthenticated user navigates to `/sessions/join`
- **THEN** the join page is shown normally without redirecting to login

#### Scenario: Unauthenticated access to view page
- **WHEN** an unauthenticated user navigates to `/sessions/view?token=<token>`
- **THEN** the view page is shown normally without redirecting to login

### Requirement: Admin can log out
The system SHALL provide a logout action. After logging out, the authentication cookie is cleared and the admin is redirected to `/login`.

#### Scenario: Successful logout
- **WHEN** an authenticated admin triggers the logout action
- **THEN** the authentication cookie is cleared
- **THEN** the admin is redirected to `/login`
