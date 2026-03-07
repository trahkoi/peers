## 1. Project Setup

- [x] 1.1 Create `tests/Peers.Architecture.Tests/Peers.Architecture.Tests.csproj` with xUnit and `TNG.ArchUnitNET.xUnit` package references, and project references to all source assemblies (Training, Spotlights, Persistence, Web)
- [x] 1.2 Add the new test project to the solution (if a .sln file exists)

## 2. Module Boundary Tests

- [x] 2.1 Implement convention-based module boundary test: discover all domain modules (Peers.* excluding Web and Persistence) and assert no domain module depends on any other domain module
- [x] 2.2 Verify tests pass with current codebase (`dotnet test` on the new project)
