## Context

The app is an ASP.NET Core (.NET 10) Razor Pages app with no external dependencies — no database, no message queue, fully in-memory. Admin credentials are passed via environment variables. The app is a single project (`Peers.Web`) with a test project (`Peers.Training.Tests`). There is no existing CI/CD pipeline and no `infra/` directory.

## Goals / Non-Goals

**Goals:**
- Define Azure infrastructure as code (Bicep) so the environment is reproducible
- Automate deployment to Azure App Service on every push to `main` via GitHub Actions
- Configure admin credentials securely as App Service application settings
- Keep the setup minimal — single slot, no staging environment

**Non-Goals:**
- Custom domain or TLS certificate management (App Service default `*.azurewebsites.net` domain is sufficient)
- Multiple environments (staging/production) — single App Service for now
- Containerisation (App Service code deploy is simpler for this app)
- Persistent storage (the app remains in-memory; restart loses session state by design)
- Azure Key Vault for secrets (App Service environment variables are sufficient at this scale)

## Decisions

### 1. Hosting: Azure App Service vs. Container Apps

**Decision:** Azure App Service (Linux, code deploy).

**Rationale:** No Dockerfile needed; `dotnet publish` artifact deploys directly. App Service has first-class .NET support, free/basic tier is affordable for a small class tool, and the setup is simpler than Container Apps.

**Alternative considered:** Azure Container Apps — more portable but requires containerisation and adds operational complexity with no benefit here.

### 2. IaC: Bicep vs. ARM vs. Terraform

**Decision:** Bicep (`infra/main.bicep`).

**Rationale:** Bicep is the native Azure IaC language, no extra tooling beyond the Azure CLI. Simpler syntax than ARM JSON. Terraform would be overkill for a single App Service.

### 3. App Service Plan tier

**Decision:** B1 (Basic, Linux).

**Rationale:** F1 (Free) has CPU quotas and no custom domain support. B1 is the minimal always-on tier. Can be parameterised to allow override.

### 4. Deployment method: GitHub Actions zip deploy vs. Azure DevOps

**Decision:** GitHub Actions with `azure/webapps-deploy` action using zip deploy.

**Rationale:** The repo is on GitHub (no Azure DevOps setup). GitHub Actions is the natural fit. Zip deploy is the simplest method — no Docker registry, no slot swaps needed.

### 5. Credentials for deployment

**Decision:** A publish profile stored as a GitHub Actions secret (`AZURE_WEBAPP_PUBLISH_PROFILE`).

**Rationale:** Simple to set up — download from Azure Portal, paste into GitHub secret. Scoped to a single app. Alternative (service principal / OIDC) is more robust for multi-resource deployments but overkill here.

### 6. Admin credentials in Azure

**Decision:** Set `AdminCredentials__Username` and `AdminCredentials__Password` as App Service application settings (environment variables).

**Rationale:** Consistent with the existing design decision to pass credentials via environment variables. App Service application settings are encrypted at rest and not visible in logs.

## Risks / Trade-offs

- **In-memory state lost on restart** → App Service restarts the process on deploy and on idle scale-down. All sessions and tokens are lost. Mitigation: document this clearly; acceptable for current scope.
- **Single slot — no zero-downtime deploy** → Deploys cause a brief restart. Mitigation: acceptable for a small class tool with low traffic.
- **Publish profile as secret** → If the secret leaks, an attacker can deploy to the app. Mitigation: rotate the publish profile if suspected leak; consider OIDC in the future.
- **B1 cost** → ~$13/month. Mitigation: parameterise the SKU so it can be downgraded to F1 for testing.

## Open Questions

- What Azure region should the App Service be deployed to? (Placeholder: `westeurope` — override via Bicep parameter.)
- What should the App Service name be? (Placeholder: `peers-app` — must be globally unique.)
