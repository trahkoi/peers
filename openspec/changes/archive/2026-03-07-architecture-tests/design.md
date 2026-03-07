## Context

The Peers solution has a modular structure with independent domain modules (`Peers.Training`, `Peers.Spotlights`) that share only `Peers.Persistence` for storage and are composed in `Peers.Web`. These boundaries exist by convention but are not enforced — violations have already slipped through.

## Goals / Non-Goals

**Goals:**
- Enforce that domain modules cannot reference each other at compile/test time
- Use ArchUnitNET for expressive, maintainable architecture rules
- Make it easy to add new modules with automatic boundary enforcement

**Non-Goals:**
- Layer enforcement (e.g., domain vs infrastructure separation within a module)
- Naming convention enforcement
- Replacing the existing API surface snapshot test (`SessionApiBoundaryTests`)

## Decisions

### 1. Dedicated test project (`Peers.Architecture.Tests`)

Architecture tests live in their own project rather than inside module-specific test projects.

**Rationale**: Architecture tests are cross-cutting — they need references to all source assemblies. Placing them in a module test project (e.g., `Peers.Training.Tests`) would create the very cross-module reference we're trying to prevent.

**Alternative considered**: Convention tests inside each module's test project checking only outbound dependencies. Rejected because it can't verify the inverse direction and requires duplicated setup.

### 2. ArchUnitNET over hand-rolled reflection

**Rationale**: ArchUnitNET provides a fluent API with clear violation messages, and scales well as we add rules. Hand-rolled reflection works for simple cases but gets verbose quickly.

**Alternative considered**: Plain `System.Reflection` assertions (like the existing `SessionApiBoundaryTests`). Viable for this single rule, but doesn't scale for future architectural constraints.

### 3. Convention-based module discovery

Define domain modules as "all assemblies under `Peers.*` except `Peers.Web` and `Peers.Persistence`". Test that no domain module depends on any other domain module. This way, adding `Peers.Billing` later automatically gets boundary enforcement without writing new tests.

**Alternative considered**: Explicit pair-wise rules (Training vs Spotlights). Simpler to read but requires manual updates for each new module.

## Risks / Trade-offs

- **[ArchUnitNET maintenance]** Another NuGet dependency to keep updated. Mitigated by it being a test-only dependency with no production impact.
- **[False sense of security]** ArchUnitNET checks type-level dependencies within loaded assemblies. It won't catch runtime coupling via shared database tables or message contracts. Mitigated by understanding the tool's scope.
- **[Convention-based discovery may be too broad]** If a future assembly like `Peers.Shared.Contracts` is added, the convention might incorrectly treat it as a domain module. Mitigated by keeping the exclusion list explicit and documented in the test.
