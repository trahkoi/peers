## Why

When a participant joins a session and later closes their browser tab, their personal token exists only in the URL — if lost, they must re-join manually. Additionally, nothing prevents the same browser from joining the same session multiple times under different names, polluting the roster.

## What Changes

- After a successful join, the participant's token and session name are persisted to `localStorage`
- On the join page, if a stored token is detected, a non-blocking notice is shown: "You're already in [session] — view it here"
- On the view page, if the token is invalid (e.g. server restarted), `localStorage` is cleared so the participant can re-join cleanly
- No server-side changes required — this is pure client-side JavaScript

## Capabilities

### New Capabilities

- `participant-token-persistence`: Store and recover a participant's personal token via browser `localStorage`, enabling token recovery after tab close and soft-blocking duplicate joins from the same browser

### Modified Capabilities

## Impact

- `Peers.Web` — JavaScript added to View and Join Razor pages (inline or via a small JS file)
- No changes to `Peers.Training` service layer
- No new endpoints or server state
