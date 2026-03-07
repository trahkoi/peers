# Peers

West Coast Swing training session management app. Coaches create sessions, dancers join via invite codes. Built with ASP.NET Core Razor Pages and SQLite.

## Project structure

```
src/
  Peers.Web/           → ASP.NET Core Razor Pages host (Program.cs, pages, auth)
  Peers.Training/      → Session management: create, join, invite codes, participants
  Peers.Spotlights/    → Spotlight pairing algorithm: pairs leaders/followers for practice rounds
  Peers.Persistence/   → Shared EF Core DbContext (SQLite), migrations, module config interface
tests/
  Peers.Training.Tests/
  Peers.Spotlights.Tests/
  Peers.Persistence.Tests/
  Peers.Architecture.Tests/  → ArchUnitNET module boundary enforcement
infra/
  main.bicep           → Azure App Service infrastructure
openspec/              → Change management (proposals, specs, tasks)
```

## Key conventions

- **Solution file**: `Peers.slnx` (XML format) — add new projects here
- **Modular architecture**: each module registers its own EF Core entity configuration via `IModuleEntityConfiguration`. Modules must not reference each other's internals (enforced by ArchUnitNET tests)
- **Persistence**: SQLite via EF Core. Connection string key is `Peers` (`ConnectionStrings:Peers`). When absent, falls back to in-memory storage
- **No accounts for dancers**: participants join via invite code and get a browser token
- **Tests**: `dotnet test` runs all test projects. Tests use in-memory SQLite

## Commands

```bash
dotnet run --project src/Peers.Web           # Run the app
dotnet test                                   # Run all tests
```

## Change management

This project uses OpenSpec for tracking changes. See `openspec/specs/` for current capability specifications. Use `/opsx:propose` to create new changes.
