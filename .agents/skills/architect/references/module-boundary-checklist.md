# Module Boundary Checklist

Use this checklist for architecture reviews and refactors.

## Module Cohesion

- Does each module have one clear responsibility?
- Are unrelated capabilities split into separate modules?
- Are module invariants explicit?

## Public API

- Is each public type or member necessary?
- Do API names describe domain behavior, not implementation?
- Are input/output contracts stable and explicit?
- Are error behaviors documented and testable?

## Encapsulation

- Are implementation types isolated behind `internal` visibility and assembly boundaries?
- Are there any references to another module's internal namespaces or types?
- Are internal types accidentally exposed through public APIs?
- Is construction complexity hidden behind factories/builders?

## Dependencies

- Is dependency direction intentional and acyclic?
- Are shared utilities truly generic, not domain leakage?
- Are cross-module calls made through contracts instead of direct data access?

## Enforcement

- Do tests fail when internal types are referenced from outside the owning assembly?
- Are analyzers/build rules configured to block forbidden project references or namespace usage?
- Is there a simple dependency map documenting allowed edges?

## Migration Safety

- Is there a clear incremental migration sequence?
- Are compatibility shims temporary and time-boxed?
- Are deprecated public APIs removed after consumer migration?
