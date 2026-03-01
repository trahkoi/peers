## Session module - v1 must-have acceptance criteria

### A) Session lifecycle

* **Create session**

  * When `CreateSession(name)` is called with a valid non-empty name, a new session is created and returns a stable `sessionId`.
  * New sessions start in `active` status.
* **End session**

  * When `EndSession(sessionId)` is called for an active session, the session status changes to `ended`.
  * Once ended, the session is immutable for attendance mutations (no joins/leaves).

### B) Dancer participation

* **Join session**

  * When `JoinSession(sessionId, dancerName, role)` is called on an active session with valid inputs, a dancer is added to that session.
  * Supported role values are exactly `leader` and `follower`.
  * Membership is unique per `(sessionId, dancerName)` for active participants; duplicate joins return a consistent result (`conflict` or idempotent success, but one behavior only).
* **Leave session**

  * When `LeaveSession(sessionId, dancerName)` is called on an active session and the dancer is currently participating, that dancer is removed from the session.
  * Repeated leave for a non-participating dancer returns a consistent result (`not_found` or idempotent success, but one behavior only).
* **List participants**

  * When `ListParticipants(sessionId)` is called, it returns active participants with at least `dancerName` and `role`.

### C) State and integrity rules

* Session names are required and trimmed; empty values are rejected.
* Dancer names are required and trimmed; empty values are rejected.
* `role` must be a valid enum value (`leader|follower`) and is stored consistently (case-normalized).
* All write operations are atomic (no partial state).

### D) Error semantics

* APIs signal errors via exceptions (not result error codes).
* Exception types are explicit and consistent for equivalent failures:

  * validation failures (invalid names/roles)
  * not found failures (unknown `sessionId`, missing participant when applicable)
  * conflict/state failures (duplicate join, invalid transition such as ending an already ended session)
* Authorization is handled outside this module and is out of scope for this spec.

### E) Public API contract (v1)

* `CreateSession(name) -> sessionId`
* `EndSession(sessionId) -> void`
* `JoinSession(sessionId, dancerName, role) -> void`
* `LeaveSession(sessionId, dancerName) -> void`
* `ListParticipants(sessionId) -> Participant[]`

That is the lean v1 core: **create session, join dancers, leave dancers, end session**.
