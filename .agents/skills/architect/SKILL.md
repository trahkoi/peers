---
name: architect
description: Modular architecture and boundary-enforcement guidance for application design and refactoring. Use when Codex needs to split systems into cohesive modules, define concise public APIs, hide implementation details, enforce dependency direction, review architecture for leakage, or produce module contracts before coding.
---

# Architect

Build and evolve software as a set of explicit modules with narrow APIs and private internals.
Default to architecture decisions that reduce coupling and make dependencies obvious.

## Core Rules

1. Define module responsibilities first; do not start with file moves.
2. Expose the smallest possible public API surface.
3. Keep domain types internal unless they are part of an intentional contract.
4. Block cross-module access to internal namespaces, types, and helper methods.
5. Keep dependency direction one-way and acyclic.
6. Prefer orchestration through interfaces/events over shared mutable state.
7. Treat leaking implementation details as an architecture defect.

## Module Design Workflow

1. Identify bounded contexts.
2. Write each module's charter in 1-2 lines: purpose, owner, and key invariants.
3. Define the public API per module:
   - inputs and outputs
   - error semantics
   - stability level (`stable`, `internal-preview`, `experimental`)
4. Move implementation behind internal namespaces and assembly boundaries.
5. Replace direct references to internals with API calls, adapters, or domain events.
6. Add boundary tests that fail on internal leakage.
7. Document allowed dependencies between modules.

## Public API Rules

- Keep APIs concise and intention-revealing.
- Export behavior, not storage or framework details.
- Use DTOs/contracts at boundaries; do not leak persistence or transport models.
- Make side effects explicit in method names and docs.
- Version APIs if breaking changes are unavoidable.

## Privacy and Encapsulation Rules

- Keep internals behind assembly boundaries and `internal` visibility.
- Ban cross-module references to another module's internal namespaces and types.
- Do not widen visibility (`internal` to `public`) to bypass module boundaries.
- Prefer factory methods/builders to hide construction complexity.
- Keep module state private; expose mutations only through API operations.

## Refactoring Strategy

1. Add module contract tests before major moves.
2. Create a compatibility shim only where needed to preserve behavior.
3. Migrate consumers in small batches.
4. Remove shims and deprecated public APIs quickly after migration.
5. Re-run architecture checks from `references/module-boundary-checklist.md`.

## Output Expectations

When asked to propose or implement architecture changes:

1. Present the target module map.
2. List each module's public API.
3. Identify current boundary violations.
4. Provide a migration plan with smallest safe increments.
5. Include tests/checks that enforce boundaries.
