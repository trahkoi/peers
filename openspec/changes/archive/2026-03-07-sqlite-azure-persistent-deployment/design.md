## Context

The Peers application uses SQLite via EF Core (`Peers.Persistence`) for session and spotlight data. The connection string is currently only defined in `appsettings.Development.json` as `Data Source=peers.db`, which creates the database in the application's working directory. On Azure App Service, each deployment replaces the application directory (`/home/site/wwwroot/`), destroying the database.

Azure App Service (Linux) provides a persistent, shared file system mounted at `/home/`. Files placed under `/home/` survive deployments and are preserved across instance restarts.

## Goals / Non-Goals

**Goals:**
- SQLite database survives Azure App Service deployments without data loss
- Zero-downtime configuration: use Azure App Settings to inject the production connection string
- Development workflow remains unchanged (relative `peers.db` path)

**Non-Goals:**
- Migrating away from SQLite to a managed database (e.g., Azure SQL, PostgreSQL)
- Backup or disaster recovery strategy for the database file
- Multi-instance support (App Service scale-out with SQLite is inherently limited)

## Decisions

### 1. Store the database under `/home/data/`

Use `Data Source=/home/data/peers.db` as the production connection string.

- `/home/` is the Azure App Service persistent mount point
- `/home/data/` is a clean subdirectory that avoids conflicts with `/home/site/` (deployment artifacts) and `/home/LogFiles/`
- The `EnsureCreated`/`Migrate` call already runs on startup, so the directory just needs to exist

**Alternative considered**: Using `/home/site/` — rejected because deployment operations touch this path.

### 2. Configure via Azure App Setting, not appsettings files

Set the connection string as an Azure App Setting (`ConnectionStrings__Peers`) rather than adding an `appsettings.Production.json` to the repo.

- Keeps secrets and environment config out of source control
- Azure App Settings override appsettings.json at runtime via the standard ASP.NET Core configuration hierarchy
- The existing `Program.cs` code (`GetConnectionString("Peers")`) picks it up without changes

**Alternative considered**: Adding `appsettings.Production.json` to the repo — rejected because it couples deployment config to source and the file would be overwritten on each deploy anyway.

### 3. Ensure the database directory exists before EF Core migration

Add a directory creation step (`Directory.CreateDirectory`) before calling `Migrate()` to handle the case where `/home/data/` doesn't exist yet on first deployment.

- `SqliteConnection` will create the `.db` file, but not parent directories
- This is a one-line defensive addition to `MigratePeersDatabase`

## Risks / Trade-offs

- **[Single-instance only]** SQLite on a shared file system does not support concurrent writes from multiple App Service instances. Mitigation: App Service plan is single-instance; document this limitation.
- **[File system reliability]** Azure's `/home/` mount uses Azure Storage (CIFS/SMB). Performance is lower than local SSD. Mitigation: acceptable for the current low-traffic use case.
- **[No automated backups]** The database file has no automated backup mechanism. Mitigation: out of scope for this change; can be added later via a scheduled Azure WebJob or manual copy.
- **[First deploy loses existing data]** If there is an existing `peers.db` in `/home/site/wwwroot/`, switching to `/home/data/peers.db` means starting fresh. Mitigation: manually copy the old file before deploying, or accept data loss if the app is new.
