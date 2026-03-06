## Why

The app currently runs only locally. Hosting it on Azure App Service makes it accessible to coaches and dancers without requiring anyone to run a server on their own machine.

## What Changes

- A Bicep template (`infra/main.bicep`) defines the Azure App Service and App Service Plan
- A GitHub Actions workflow (`.github/workflows/deploy.yml`) builds and deploys the app to Azure on every push to `main`
- Admin credentials are configured as App Service application settings (environment variables), not in source code
- README is updated with deployment instructions

## Capabilities

### New Capabilities

- `azure-deployment`: The app is continuously deployed to Azure App Service via GitHub Actions, with infrastructure defined as code in Bicep

### Modified Capabilities

## Impact

- New `infra/` directory with Bicep template
- New `.github/workflows/` directory with deployment workflow
- `README.md` updated with deployment and configuration instructions
- No changes to application code
- Requires an Azure subscription and a GitHub repository with Actions enabled
