## Context

Coaches generate spotlight rounds to pair leaders and followers for practice. The system supports multiple rounds per session, but once generated, a round cannot be removed. Coaches need the ability to delete a round (e.g., generated with wrong participants).

Cascade delete from `SpotlightRoundEntity` to `PairingEntity` is already configured in `SpotlightsEntityConfiguration`.

## Goals / Non-Goals

**Goals:**
- Allow deleting a spotlight round by its ID
- Cascade-delete associated pairings automatically
- Expose delete action on the Manage page for coaches

**Non-Goals:**
- Renumbering remaining rounds after deletion
- Undo/soft-delete functionality
- Confirmation dialog (keep it simple; rely on flash message feedback)

## Decisions

### 1. Delete by round ID (not round number)

Round IDs are globally unique and unambiguous. Round numbers could have gaps after deletion, making them less reliable as identifiers. Using the round ID avoids confusion.

### 2. Rely on existing cascade delete

The EF Core configuration already has `OnDelete(DeleteBehavior.Cascade)` for pairings. Removing the `SpotlightRoundEntity` will automatically remove its pairings — no additional deletion logic needed.

### 3. No round renumbering after deletion

Renumbering would add complexity and could confuse coaches who remember "Round 3" — suddenly it becomes "Round 2". Gaps in round numbers are acceptable and expected.

### 4. Coach-only authorization

Follow the same pattern as `OnPostGenerateSpotlight` — any authenticated admin or participant-coach can delete rounds. No additional authorization layer needed.

## Risks / Trade-offs

- **Accidental deletion** → Mitigated by flash message confirming which round was deleted. A confirmation dialog could be added later if needed.
- **Historical pairing data lost** → Deleted pairings no longer contribute to the pairing algorithm's historical counts. This is intentional — if a round is deleted, its pairings should not influence future rounds.
