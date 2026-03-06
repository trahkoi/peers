## Context

Both the Join page and the Manage page "Add dancer" form use `<select asp-for="Role">` to render a dropdown for the Leader/Follower choice. The site uses a custom design system in `site.css` with CSS variables, rounded borders, and a warm color palette. There is no JavaScript framework — forms use standard HTML with a lightweight `js-busy-form` pattern.

## Goals / Non-Goals

**Goals:**
- Replace the dropdown with a two-button toggle that visually highlights the selected role
- Keep the form binding working via a hidden `<input>` for the `Role` field
- Match the existing design system (border-radius, colors, font weight)

**Non-Goals:**
- Changing the `SessionRole` enum or backend validation
- Adding JavaScript beyond the minimum needed to toggle selection state

## Decisions

**1. Hidden input + button pair pattern**

Use a hidden `<input name="Role">` and two `<button type="button">` elements styled as a toggle group. Clicking a button updates the hidden input value and toggles an `active` class via inline `onclick`. This avoids radio button styling complexity while keeping the form submission simple.

Alternative considered: styled radio buttons with `appearance: none`. Works but requires more CSS to fully override default radio behavior across browsers. The hidden input approach is simpler and more predictable.

**2. Leader selected by default**

The current `<select>` defaults to Leader (first enum value). The toggle will do the same — Leader button starts with the `active` class and the hidden input starts with value `0`.

**3. Single `.role-toggle` CSS component**

Add a small reusable component class in `site.css`. The toggle uses the existing `--accent` color for the active state and `--line` border for the inactive state, staying consistent with the design system.

## Risks / Trade-offs

- **[Risk] No-JS fallback** → If JavaScript is disabled, the hidden input defaults to Leader and the buttons do nothing. This is acceptable because: (a) the existing `js-busy-form` pattern already requires JS, and (b) Leader is a reasonable default.
