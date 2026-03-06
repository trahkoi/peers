# Peers

A web app for managing West Coast Swing training session attendance. Coaches create sessions and track participants; dancers join via a shareable invite code — no account required.

## Running the app

```bash
dotnet run --project src/Peers.Web
```

### Admin credentials

The session management pages require a coach login. Credentials are **not** stored in `appsettings.json` — pass them as environment variables:

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
