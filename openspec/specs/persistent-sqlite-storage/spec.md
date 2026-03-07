## ADDED Requirements

### Requirement: Database directory is created before migration
The system SHALL ensure that the parent directory of the SQLite database file exists before applying EF Core migrations. If the directory does not exist, the system SHALL create it.

#### Scenario: First deployment to a fresh environment
- **WHEN** the application starts and the database directory (e.g., `/home/data/`) does not exist
- **THEN** the system SHALL create the directory before calling `Database.Migrate()`
- **THEN** the migration SHALL complete successfully and the database file SHALL be created

#### Scenario: Subsequent deployments with existing directory
- **WHEN** the application starts and the database directory already exists
- **THEN** the system SHALL not fail and SHALL proceed with migration as normal

### Requirement: Production connection string points to persistent storage
The system SHALL be configurable with a connection string that points the SQLite database to a path outside the application deployment directory. On Azure App Service, this SHALL be a path under the `/home/` persistent mount.

#### Scenario: Azure App Setting provides connection string
- **WHEN** the Azure App Setting `ConnectionStrings__Peers` is set to `Data Source=/home/data/peers.db`
- **THEN** the application SHALL use `/home/data/peers.db` as the database file
- **THEN** the database SHALL persist across application deployments

#### Scenario: Development uses relative path
- **WHEN** the connection string is `Data Source=peers.db` (from `appsettings.Development.json`)
- **THEN** the application SHALL create the database in the working directory
- **THEN** development workflow SHALL remain unchanged
