## Group module — v1 must-have acceptance criteria

### A) Group lifecycle

* **Create group**

  * When `CreateGroup(name, actorId)` is called with a valid non-empty name, a new group is created and returns a stable `groupId`.
* **Get group**

  * When `GetGroup(groupId)` is called, it returns group metadata (at least `groupId`, `name`) or “not found”.
* **Update group**

  * When `UpdateGroup(groupId, name, actorId)` is called by an authorized actor, editable fields (at least `name`) are updated; invalid updates are rejected.

### B) Membership (roster) management

* **Add member**

  * When `AddMember(groupId, memberId, role, actorId)` is called by an authorized actor, membership is created.
  * Membership is unique per `(groupId, memberId)`; repeat adds have defined behavior (either no-op or conflict error, but consistent).
* **Remove member**

  * When `RemoveMember(groupId, memberId, actorId)` is called by an authorized actor, the member is removed from the group.
  * Constraint enforced: cannot remove the last admin/owner (whatever the highest role is).
* **Change role**

  * When `ChangeMemberRole(groupId, memberId, newRole, actorId)` is called by an authorized actor, the role updates.
  * Constraint enforced: cannot demote the last admin/owner.
* **List roster**

  * When `ListMembers(groupId)` is called, it returns members with at least `memberId`, display name (or reference), and `role`.

### C) Authorization (minimal RBAC)

* The module enforces permissions on all mutations:

  * Only admins (or admins+coaches, if you choose) can update group settings and manage members.
* Unauthorized calls return “forbidden” consistently.

### D) Contract for other modules (Session module dependency)

* **Roster query contract**

  * `GetRoster(groupId)` (or `ListMembers`) returns stable `memberId`s and active membership info that Session can use to:

    * populate attendance UI
    * validate “is this person in this group?”
### E) Data integrity & API behavior

* Inputs validated (non-empty names; valid role enum; IDs well-formed).
* Errors are consistent across APIs: `not_found`, `validation_error`, `forbidden`, `conflict`.
* All writes are atomic (no partial state).

That’s the lean v1 core: **groups + roster + RBAC + a stable roster contract**.
