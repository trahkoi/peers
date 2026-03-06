# Peers

A web app for managing West Coast Swing training session attendance. Coaches create sessions and track participants; dancers join via a shareable invite code — no account required.

## Running the app

```bash
dotnet run --project src/Peers.Web
```

### Admin credentials

The session management pages require a coach login. They can be passed as environment variables:

```bash
AdminCredentials__Username=coach AdminCredentials__Password=yourpassword dotnet run --project src/Peers.Web
```

Or export them first:

```bash
export AdminCredentials__Username=coach
export AdminCredentials__Password=yourpassword
dotnet run --project src/Peers.Web
```

The double-underscore (`__`) is the ASP.NET Core convention for nested config keys and maps to `AdminCredentials.Username` / `AdminCredentials.Password` in `appsettings.json`.

## Running tests

```bash
dotnet test
```

## Deploying to Azure

The app deploys to Azure App Service via GitHub Actions on every push to `main`. Infrastructure is defined in `infra/main.bicep`.

### Prerequisites

- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) installed and logged in (`az login`)
- An Azure subscription and resource group
- A GitHub repository with Actions enabled

### 1. Provision infrastructure

```bash
az deployment group create \
  --resource-group <your-resource-group> \
  --template-file infra/main.bicep \
  --parameters appName=<your-unique-app-name>
```

The `appName` must be globally unique — it becomes `<appName>.azurewebsites.net`.

### 2. Set admin credentials on the App Service

```bash
az webapp config appsettings set \
  --resource-group <your-resource-group> \
  --name <your-unique-app-name> \
  --settings AdminCredentials__Username=<username> AdminCredentials__Password=<password>
```

### 3. Configure GitHub Actions secrets and variables

In your GitHub repository settings:

| Type | Name | Value |
|------|------|-------|
| Variable | `AZURE_WEBAPP_NAME` | Your App Service name (e.g. `peers-app`) |
| Secret | `AZURE_WEBAPP_PUBLISH_PROFILE` | Contents of the publish profile downloaded from Azure Portal → App Service → **Get publish profile** |

### 4. Deploy

Push to `main`. GitHub Actions will build, test, and deploy automatically. The app will be available at `https://<appName>.azurewebsites.net`.

> **Note:** The app is in-memory — all sessions and tokens are lost on restart or redeploy.
