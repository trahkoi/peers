## Why

The SQLite database file (`peers.db`) is stored at a relative path inside the application directory. On Azure App Service, each deployment replaces the application directory contents, destroying the database and all persisted data (sessions, spotlights, dancer identities). The database must survive deployments.

## What Changes

- Configure the SQLite database path to use Azure App Service's persistent storage (`/home/` mount) in production
- Add a production connection string configuration that points to the persistent file system
- Ensure the deployment workflow does not interfere with the database file

## Capabilities

### New Capabilities
- `persistent-sqlite-storage`: Configure SQLite to use a deployment-safe file path on Azure App Service, ensuring the database persists across deployments

### Modified Capabilities
- `sqlite-session-storage`: The connection string resolution changes to support environment-specific paths (persistent storage in production vs. relative path in development)

## Impact

- **Configuration**: `appsettings.json` or Azure App Settings must define a connection string pointing to `/home/data/peers.db` (or similar persistent path)
- **Deployment**: The GitHub Actions workflow may need to set the connection string via Azure App Settings rather than relying on appsettings files
- **Code**: `Program.cs` connection string resolution is already dynamic; minimal code changes expected
- **Data**: Existing production data (if any) will be lost on the first deploy with the new path unless manually migrated
