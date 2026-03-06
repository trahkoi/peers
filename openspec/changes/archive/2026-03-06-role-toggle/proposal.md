## Why

The role selection on both the Join and Manage pages uses a `<select>` dropdown for a binary choice (Leader / Follower). A dropdown adds unnecessary interaction cost — it requires a click to open, visual scanning, and a second click to select. A two-option toggle is faster, shows both options at a glance, and better communicates that there are exactly two roles.

## What Changes

- Replace the `<select>` dropdown for role selection with a two-button toggle on the **Join** page (`/sessions/join`)
- Replace the same `<select>` dropdown with a two-button toggle on the **Manage** page (`/sessions/{id}`, "Add dancer" form)
- Add CSS for the toggle component in `site.css`
- The underlying form value remains `SessionRole` enum (0 = Leader, 1 = Follower) — no backend changes

## Capabilities

### New Capabilities

_(none)_

### Modified Capabilities

_(none — this is a presentation-only change; the role value and form binding are unchanged)_

## Impact

- `Join.cshtml` — replace `<select>` with toggle markup
- `Manage.cshtml` — replace `<select>` with toggle markup
- `site.css` — add `.role-toggle` styles
- No backend, model, or service changes
