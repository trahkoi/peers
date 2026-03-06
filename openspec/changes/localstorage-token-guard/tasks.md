## 1. View Page — Persist token to localStorage

- [x] 1.1 On `/sessions/view`, add inline JS that writes `{ token, sessionName }` to `localStorage["peers_participant_token"]` when the page loads with a valid token (token is in URL and session is found)
- [x] 1.2 Wrap the localStorage write in a try/catch so failures are silent

## 2. View Page — Clear localStorage on invalid token

- [x] 2.1 On `/sessions/view`, in the "token not found" state, add inline JS that removes `localStorage["peers_participant_token"]`

## 3. Join Page — Read localStorage and show notice

- [x] 3.1 On `/sessions/join`, add inline JS that reads `localStorage["peers_participant_token"]` on page load
- [x] 3.2 If a stored token is found, inject a notice element showing "You're already in [sessionName] — view it here" with a link to `/sessions/view?token=<token>`
- [x] 3.3 Wrap the localStorage read in a try/catch so failures are silent and the notice is simply not shown
