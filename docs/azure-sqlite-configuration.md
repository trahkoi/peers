# Azure SQLite Configuration

The application uses SQLite for persistence. On Azure App Service, the database must be stored on the persistent `/home/` mount to survive deployments.

## Required App Setting

Set the following connection string in Azure App Service **Configuration > Connection strings**:

| Name     | Value                            | Type   |
|----------|----------------------------------|--------|
| Peers | `Data Source=/home/data/peers.db` | SQLite |

Alternatively, set it via Azure CLI:

```bash
az webapp config connection-string set \
  --resource-group <resource-group> \
  --name <app-name> \
  --connection-string-type SQLite \
  --settings Peers="Data Source=/home/data/peers.db"
```

Or as an app setting (environment variable format):

```bash
az webapp config appsettings set \
  --resource-group <resource-group> \
  --name <app-name> \
  --settings ConnectionStrings__Peers="Data Source=/home/data/peers.db"
```

## How it works

- Azure App Settings override `appsettings.json` values at runtime
- The app creates the `/home/data/` directory automatically on first startup
- The `/home/` mount persists across deployments and restarts
- No changes to the deployment workflow are needed
