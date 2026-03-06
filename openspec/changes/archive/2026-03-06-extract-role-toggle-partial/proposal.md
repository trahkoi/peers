## Why

The role toggle markup (hidden input + two toggle buttons + inline JS) is duplicated verbatim in `Join.cshtml` and `Manage.cshtml`. Extracting it into a Razor partial view eliminates the duplication and makes future changes to the toggle (styling, additional roles, accessibility) a single-point edit.

## What Changes

- Create a `_RoleToggle.cshtml` partial view in `Pages/Sessions/`
- Replace the inline toggle markup in `Join.cshtml` with a `<partial>` tag
- Replace the inline toggle markup in `Manage.cshtml` with a `<partial>` tag
- No backend changes — the partial assumes the page model has an `asp-for`-compatible `Role` property

## Capabilities

### New Capabilities

_(none)_

### Modified Capabilities

_(none — this is a refactoring with no behavior change)_

## Impact

- `Pages/Sessions/Join.cshtml` — replace toggle markup with partial reference
- `Pages/Sessions/Manage.cshtml` — replace toggle markup with partial reference
- `Pages/Sessions/_RoleToggle.cshtml` — new file containing the extracted toggle
