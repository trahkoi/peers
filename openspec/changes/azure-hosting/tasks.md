## 1. Bicep Infrastructure

- [x] 1.1 Create `infra/main.bicep` with parameters for `appName`, `location`, and `appServicePlanSku` (default B1)
- [x] 1.2 Define App Service Plan resource (Linux, parameterised SKU)
- [x] 1.3 Define App Service resource targeting .NET 10 on Linux, with `WEBSITE_RUN_FROM_PACKAGE=1` and placeholder app settings for `AdminCredentials__Username` and `AdminCredentials__Password`
- [x] 1.4 Output the App Service default hostname from the Bicep template

## 2. GitHub Actions Workflow

- [x] 2.1 Create `.github/workflows/deploy.yml` triggered on push to `main`
- [x] 2.2 Add a build-and-test job: `dotnet restore`, `dotnet build`, `dotnet test`
- [x] 2.3 Add a deploy job (depends on build-and-test): `dotnet publish` then deploy using `azure/webapps-deploy` action with `AZURE_WEBAPP_PUBLISH_PROFILE` secret
- [x] 2.4 Parameterise the workflow with the App Service name (via env var or workflow input)

## 3. README

- [x] 3.1 Add a "Deploying to Azure" section to `README.md` covering: prerequisites (Azure CLI, az login), provisioning with `az deployment group create`, downloading the publish profile, and setting GitHub Actions secrets (`AZURE_WEBAPP_PUBLISH_PROFILE`, `AZURE_WEBAPP_NAME`)
- [x] 3.2 Document setting `AdminCredentials__Username` and `AdminCredentials__Password` as App Service application settings after provisioning
