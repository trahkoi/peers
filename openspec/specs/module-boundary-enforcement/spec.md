### Requirement: Domain modules SHALL NOT depend on each other
The architecture test suite SHALL verify that no domain module assembly has type-level dependencies on any other domain module assembly. Domain modules are defined as all `Peers.*` assemblies excluding `Peers.Web` and `Peers.Persistence`.

#### Scenario: Training does not reference Spotlights
- **WHEN** the architecture tests analyze `Peers.Training` assembly
- **THEN** no types in `Peers.Training` SHALL depend on any types in `Peers.Spotlights`

#### Scenario: Spotlights does not reference Training
- **WHEN** the architecture tests analyze `Peers.Spotlights` assembly
- **THEN** no types in `Peers.Spotlights` SHALL depend on any types in `Peers.Training`

#### Scenario: New module added without updating tests
- **WHEN** a new domain module `Peers.Billing` is added and referenced by the architecture test project
- **THEN** the convention-based rule SHALL automatically enforce that `Peers.Billing` does not depend on `Peers.Training` or `Peers.Spotlights` (and vice versa) without requiring new test code

### Requirement: Domain modules MAY depend on shared infrastructure
Domain modules SHALL be allowed to depend on `Peers.Persistence` and on general .NET framework packages. The architecture tests SHALL NOT flag these as violations.

#### Scenario: Training references Persistence
- **WHEN** the architecture tests analyze `Peers.Training` assembly
- **THEN** dependencies on types in `Peers.Persistence` SHALL NOT be flagged as violations

### Requirement: Architecture tests run as standard xUnit tests
The architecture tests SHALL be implemented as xUnit test cases using the `TNG.ArchUnitNET.xUnit` integration, so they execute as part of `dotnet test` with no additional tooling.

#### Scenario: Architecture tests execute with dotnet test
- **WHEN** a developer runs `dotnet test` on the solution
- **THEN** the module boundary architecture tests SHALL execute alongside all other tests
