## ADDED Requirements

### Requirement: Infrastructure is defined as Bicep
The repository SHALL contain a Bicep template at `infra/main.bicep` that defines all Azure resources required to host the app. Running the template SHALL be sufficient to provision a working environment.

#### Scenario: Bicep template provisions App Service and plan
- **WHEN** the Bicep template is deployed to an Azure resource group
- **THEN** an App Service Plan (Linux, B1) is created
- **THEN** an App Service targeting .NET 10 on Linux is created within that plan
- **THEN** the App Service is configured to run the `Peers.Web` application

#### Scenario: Bicep template accepts parameters for customisation
- **WHEN** the template is deployed with custom parameter values
- **THEN** the App Service name and region reflect the provided values

### Requirement: App is deployed to Azure on every push to main
A GitHub Actions workflow SHALL build the application, run tests, and deploy to Azure App Service on every push to the `main` branch.

#### Scenario: Successful push triggers deploy
- **WHEN** a commit is pushed to the `main` branch
- **THEN** GitHub Actions builds the app with `dotnet publish`
- **THEN** tests are run and must pass before deployment proceeds
- **THEN** the published artifact is deployed to Azure App Service

#### Scenario: Failed tests block deployment
- **WHEN** a commit is pushed to `main` and tests fail
- **THEN** the deployment step is skipped
- **THEN** the workflow reports failure

### Requirement: Admin credentials are configured as App Service environment variables
The Azure App Service SHALL have `AdminCredentials__Username` and `AdminCredentials__Password` configured as application settings so the admin login works without credentials in source code.

#### Scenario: App starts with credentials from App Service settings
- **WHEN** the app starts on Azure App Service with application settings set
- **THEN** the admin login page accepts the configured username and password
- **THEN** credentials are not present in `appsettings.json` or the deployment artifact

### Requirement: Deployment instructions are documented in README
The README SHALL contain a section explaining how to provision the Azure infrastructure and configure required secrets for GitHub Actions.

#### Scenario: New developer can deploy from README
- **WHEN** a developer follows the README deployment section
- **THEN** they can provision the Azure infrastructure using the Bicep template
- **THEN** they can configure GitHub Actions secrets for automated deployment
- **THEN** pushing to `main` triggers a successful deployment
