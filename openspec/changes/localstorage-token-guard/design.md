## Context

Participants join a training session via invite code and receive a personal token embedded in their view URL (`/sessions/view?token=...`). This token is the sole means of re-entering the session view. If the tab is closed and the URL is not saved, the token is lost and the participant must re-join — which works, but is not obvious.

Additionally, the join form has no browser-side guard preventing the same person from joining multiple times under different names. While the admin can clean this up manually, it degrades the roster.

The token is already issued by the server and surfaced in the URL. No server changes are needed — we just need the browser to remember it.

## Goals / Non-Goals

**Goals:**
- Persist participant token to `localStorage` after a successful join
- Show a non-blocking notice on the join page when a stored token is found
- Clear `localStorage` when a stored token is found to be invalid (server restarted)
- Allow participants to naturally recover their session view without re-joining

**Non-Goals:**
- Preventing joins from incognito mode or different devices (hard barrier, not soft nudge)
- Server-side token validation on the join page (adds a round-trip; not worth it)
- Multi-session token storage (single slot is sufficient — one active session at a time)

## Decisions

### 1. Storage format: single slot vs. keyed by session

**Decision:** Single slot — `peers_participant_token` key stores `{ token, sessionName }`.

**Rationale:** A participant is in at most one session at a time. Keying by session ID or invite code would add complexity with no real benefit. Joining a new session naturally overwrites the old entry.

**Alternative considered:** Key by invite code — more complex, no benefit for this use case.

### 2. What to store alongside the token

**Decision:** Store `{ token: "<guid>", sessionName: "<string>" }`.

**Rationale:** `sessionName` lets the join page show "You're already in [Tuesday Class]" without a server round-trip. The token alone would require a fetch to know what session to name in the notice.

### 3. Notice vs. auto-redirect on join page

**Decision:** Show a non-blocking dismissible notice — do not auto-redirect.

**Rationale:** The participant may be intentionally joining a *different* session. Auto-redirecting would hijack their navigation. A notice respects their intent while surfacing the stored token.

### 4. Where to write to localStorage

**Decision:** Write from the view page (`/sessions/view`) after a successful token lookup, not from the join page.

**Rationale:** The join page does a server POST and redirects — there's no clean moment to write client-side before the redirect. The view page loads with the token already in the URL and the session already resolved, making it the right place to persist.

### 5. Stale token handling

**Decision:** The view page distinguishes two failure states: `TokenMissing` (no token in the URL) and `TokenInvalid` (token presented to the server but not recognised). `localStorage` is cleared only on `TokenInvalid`.

**Rationale:** If the server restarts, the stored token is invalid — clearing it breaks the loop (join page notice → view → not found → join page notice → ...) and lets the participant re-join cleanly. On `TokenMissing`, however, the stored token may still be valid; clearing it would destroy a working recovery path. Instead, the `TokenMissing` state triggers a JS redirect to `/sessions/view?token=<stored>` if localStorage has an entry, letting the participant land in their session transparently.

## Risks / Trade-offs

- **Soft barrier only** → A determined participant can open incognito or a different browser to join again. Mitigation: acceptable — we're targeting casual/accidental misuse, not adversarial.
- **Single-slot overwrites** → Joining a new session clears the old token. Mitigation: expected behavior; participants are in one session at a time.
- **localStorage unavailable** → Private browsing in some browsers, or strict privacy settings, may block `localStorage`. Mitigation: all writes/reads wrapped in try/catch; feature degrades silently.
