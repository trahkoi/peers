## Context

The role toggle was introduced in the `role-toggle` change. Both `Join.cshtml` and `Manage.cshtml` contain identical markup: a hidden `<input>` for `Role`, a `.role-toggle` div with two buttons, and inline `onclick` handlers. Both page models bind to `SessionRole Role`.

## Goals / Non-Goals

**Goals:**
- Extract the duplicated toggle into a single Razor partial view
- Keep the partial simple — no model class, no view component overhead

**Non-Goals:**
- Changing the toggle behavior, styling, or JS
- Generalizing the partial for other enum types

## Decisions

**1. Partial view at `Pages/Sessions/_RoleToggle.cshtml`**

Place the partial alongside its consumers rather than in `Pages/Shared/`. It's only used by session pages, so colocating it keeps the relationship obvious. If it's ever needed elsewhere, it can be moved to `Shared/` later.

Alternative considered: `Pages/Shared/Components/` or a View Component. Rejected — no server-side logic to encapsulate, and a partial is the simplest Razor mechanism for shared markup.

**2. Use `<partial name="_RoleToggle" />` with implicit model**

Both pages have a `Role` property of type `SessionRole`. The partial will use `asp-for="Role"` directly, relying on the ambient `ViewData` model. This keeps the call site clean with no explicit model passing.

## Risks / Trade-offs

- **[Risk] Implicit coupling to `Role` property name** → Acceptable since both consumers already have this property. If a future page uses a different property name, it can pass a model or we revisit the approach then.
