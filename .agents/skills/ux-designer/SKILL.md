---
name: ux-designer
description: UX design guidance for frontend work focused on appealing, natural, and minimal user flows. Use when designing or reviewing page structure, user journeys, forms, navigation, interaction states, and information architecture for web UIs. Default to static or server-side rendered HTML with progressive enhancement and minimal JavaScript; escalate to richer interaction only when the task needs client-side state, high-frequency interactions, or real-time feedback.
---

# UX Designer

Design flows that feel obvious, calm, and fast.
Start with clear user intent, keep paths short, and avoid unnecessary interaction complexity.

## Workflow

1. Define the outcome and user intent.
2. Map the shortest end-to-end flow (entry, progress, success, recovery).
3. Remove non-essential steps, fields, and decisions.
4. Choose rendering strategy with an SSR-first bias.
5. Design interaction states (empty, loading, success, error, validation).
6. Validate accessibility, responsiveness, and performance constraints.
7. Ship a simple baseline first, then enhance only where user value is clear.

## Rendering Strategy (Default)

- Default to static HTML or server-side rendered HTML.
- Keep core navigation and task completion usable without JavaScript.
- Add JavaScript as progressive enhancement for targeted improvements.
- Prefer server actions/forms over client-managed orchestration when both solve the same problem.

Use richer client-side UX only when at least one condition is true:
- Interaction needs instant local feedback at high frequency (for example drawing, drag-and-drop boards, complex filters).
- The UI has complex transient state that would be awkward or brittle with full server round-trips.
- Real-time collaboration/presence materially changes user value.
- Offline-first behavior is a hard requirement.

If rich UX is required:
- Hydrate only interactive islands/components, not entire pages by default.
- Defer non-critical scripts and keep JS payloads small.
- Preserve semantic HTML and keyboard access before adding enhancement layers.

## UX Principles

### Make Flows Natural
- Match user mental models and familiar language.
- Reveal information progressively: only what is needed now.
- Keep one primary action per screen region.
- Reduce context switching and backtracking.

### Keep It Minimal
- Remove decorative complexity that does not improve comprehension or confidence.
- Prefer clear hierarchy, spacing, and typography over heavy chrome.
- Keep choice sets small; provide sane defaults.
- Split long tasks into meaningful steps only when it reduces cognitive load.

### Keep It Trustworthy
- Show system status quickly (loading, saved, failed, next step).
- Prevent errors with constraints, examples, and inline validation.
- Provide recovery paths: undo, edit, retry, and clear error copy.

## Frontend Output Contract

When asked to design or review frontend UX, produce:

1. User goal and success criteria.
2. Flow map with explicit steps and decision points.
3. Screen/state inventory:
   - default
   - empty
   - loading
   - success
   - error/validation
4. Rendering plan:
   - what is static/SSR
   - what is progressively enhanced with JS
   - what (if anything) needs rich client behavior and why
5. Accessibility checklist (semantic structure, keyboard, focus, contrast, labels, errors).
6. Performance checklist (initial payload, JS budget, interaction responsiveness).
7. Clear implementation notes for frontend engineers.

## Heuristics For Decisions

Choose simple defaults first:
- Server-render first meaningful content.
- Use plain forms and links before JS-driven equivalents.
- Prefer resilient controls over novel interactions.

Escalate complexity only with evidence:
- User research indicates friction with baseline flow.
- Metrics show bottlenecks (drop-off, task time, error rate, poor INP/LCP/CLS).
- The richer pattern improves task success, not just visual polish.

## References

For concise external guidance and source links, read:
- `references/modern-ux-guidelines.md`
