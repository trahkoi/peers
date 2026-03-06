## 1. Bicep Infrastructure

- [ ] 1.1 Create `infra/main.bicep` with parameters for `appName`, `location`, and `appServicePlanSku` (default B1)
- [ ] 1.2 Define App Service Plan resource (Linux, parameterised SKU)
- [ ] 1.3 Define App Service resource targeting .NET 10 on Linux, with `WEBSITE_RUN_FROM_PACKAGE=1` and placeholder app settings for `AdminCredentials__Username` and `AdminCredentials__Password`
- [ ] 1.4 Output the App Service default hostname from the Bicep template

## 2. GitHub Actions Workflow

- [ ] 2.1 Create `.github/workflows/deploy.yml` triggered on push to `main`
- [ ] 2.2 Add a build-and-test job: `dotnet restore`, `dotnet build`, `dotnet test`
- [ ] 2.3 Add a deploy job (depends on build-and-test): `dotnet publish` then deploy using `azure/webapps-deploy` action with `AZURE_WEBAPP_PUBLISH_PROFILE` secret
- [ ] 2.4 Parameterise the workflow with the App Service name (via env var or workflow input)

## 3. README

- [ ] 3.1 Add a "Deploying to Azure" section to `README.md` covering: prerequisites (Azure CLI, az login), provisioning with `az deployment group create`, downloading the publish profile, and setting GitHub Actions secrets (`AZURE_WEBAPP_PUBLISH_PROFILE`, `AZURE_WEBAPP_NAME`)
- [ ] 3.2 Document setting `AdminCredentials__Username` and `AdminCredentials__Password` as App Service application settings after provisioning
