## 1. Ensure database directory exists

- [x] 1.1 In `MigratePeersDatabase`, extract the database file path from the connection string and call `Directory.CreateDirectory` on its parent directory before `db.Database.Migrate()`
- [x] 1.2 Add a test verifying that `MigratePeersDatabase` succeeds when the target directory does not yet exist

## 2. Production connection string configuration

- [x] 2.1 Add `ConnectionStrings:Sessions` with value `Data Source=/home/data/peers.db` to Azure App Service App Settings (or document the required setting)
- [x] 2.2 Verify the deployment workflow (`deploy.yml`) does not need changes — the connection string comes from Azure App Settings, not from appsettings files

## 3. Validation

- [ ] 3.1 Deploy to Azure and confirm the database is created at `/home/data/peers.db`
- [ ] 3.2 Trigger a second deployment and confirm the database file and its data survive
