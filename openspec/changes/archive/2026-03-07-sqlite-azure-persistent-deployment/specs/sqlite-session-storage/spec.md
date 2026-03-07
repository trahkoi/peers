## MODIFIED Requirements

### Requirement: Application selects persistence mode via configuration
The system SHALL use SQLite persistence when a `Sessions` connection string is configured. When no connection string is present, the system SHALL fall back to the in-memory implementation. The connection string value SHALL support both relative paths (development) and absolute paths (production/Azure).

#### Scenario: Connection string present selects SQLite
- **WHEN** the application starts with a `ConnectionStrings:Peers` configuration value
- **THEN** the system registers `SqliteSessionService` as the `ISessionService` implementation
- **THEN** EF Core migrations are applied automatically on startup
- **THEN** the parent directory of the database file SHALL be created if it does not exist

#### Scenario: No connection string falls back to in-memory
- **WHEN** the application starts without a `ConnectionStrings:Peers` configuration value
- **THEN** the system registers `InMemorySessionService` as the `ISessionService` implementation
- **THEN** no database file is created

#### Scenario: Absolute path connection string
- **WHEN** the connection string is `Data Source=/home/data/peers.db`
- **THEN** the system SHALL use the absolute path for the database file
- **THEN** the database SHALL be stored at `/home/data/peers.db`
