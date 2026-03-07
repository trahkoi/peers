## Why

Domain modules (Training, Spotlights) must remain independent of each other. Without automated enforcement, cross-module references can slip in unnoticed — as already happened when Claude introduced a module-to-module dependency. ArchUnitNET architecture tests catch these violations at build time, making the modular boundaries a living, testable contract.

## What Changes

- Add a new `Peers.Architecture.Tests` project with ArchUnitNET
- Add architecture tests that enforce module isolation: Training and Spotlights must not reference each other
- Only `Peers.Web` (composition root) and `Peers.Persistence` (shared infrastructure) are allowed as cross-module dependencies

## Capabilities

### New Capabilities
- `module-boundary-enforcement`: Architecture tests using ArchUnitNET that verify domain modules do not depend on each other. Covers assembly-level and namespace-level dependency rules between Training and Spotlights.

### Modified Capabilities

None.

## Impact

- **New project**: `tests/Peers.Architecture.Tests` — references all source assemblies for inspection
- **New dependency**: `TNG.ArchUnitNET.xUnit` NuGet package
- **CI**: Architecture tests run as part of the regular `dotnet test` — no special configuration needed
- **No production code changes**
