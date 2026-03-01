# Modern UX Guidelines (SSR-First)

Use this file for quick, evidence-based design checks when building frontend flows.

## Core Guidance

1. Prefer progressive enhancement:
   - Start with semantic HTML that works without JavaScript.
   - Layer JS interactions on top without breaking core task completion.
2. Keep journeys short and obvious:
   - Clarify next action at each step.
   - Reduce unnecessary fields and decisions.
3. Design all key states:
   - default, empty, loading, success, validation, error.
4. Optimize perceived and real speed:
   - Deliver useful SSR content quickly.
   - Delay or avoid non-essential client JavaScript.
5. Build accessibility in from the start:
   - Semantic structure, keyboard support, focus handling, labels, and readable feedback.

## When Rich UX Is Worth It

Use richer client-side patterns when they clearly improve outcomes:
- High-frequency direct manipulation (drag/drop, canvas-like interactions).
- Complex local state with many transient edits.
- Real-time collaboration, presence, or live updates.
- Offline-first requirements.

If none apply, keep SSR + minimal JS.

## Practical Checklists

### Flow Quality
- Is the user goal explicit?
- Is the primary action visually dominant?
- Is there a recovery path for each failure mode?
- Can a first-time user complete this without instructions?

### Frontend Architecture
- Can core flow complete with links/forms and server responses?
- Is JS limited to components that truly need it?
- Is hydration scope minimized?
- Are loading and error states visible and specific?

### Accessibility
- Landmarks and heading hierarchy are meaningful.
- Focus order and visible focus indicator are correct.
- Inputs have labels, help text, and actionable error messages.
- Color is not the only channel for meaning.

## Sources

- W3C WAI, "Accessibility Principles" (POUR): https://www.w3.org/WAI/fundamentals/accessibility-principles/
- MDN, "Progressive enhancement": https://developer.mozilla.org/en-US/docs/Glossary/Progressive_Enhancement
- web.dev, "How browsers work" (rendering fundamentals): https://web.dev/howbrowserswork/
- GOV.UK Service Manual, "Make your service accessible and inclusive": https://www.gov.uk/service-manual/helping-people-to-use-your-service/making-your-service-accessible-an-introduction
- Nielsen Norman Group, "10 Usability Heuristics for User Interface Design": https://www.nngroup.com/articles/ten-usability-heuristics/
