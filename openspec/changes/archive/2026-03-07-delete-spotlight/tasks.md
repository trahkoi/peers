## 1. Service Layer

- [x] 1.1 Add `DeleteRound(Guid roundId)` method to `ISpotlightService`
- [x] 1.2 Implement `DeleteRound` in `SpotlightService` — find and remove the round entity, rely on cascade delete for pairings

## 2. Web Layer

- [x] 2.1 Add `OnPostDeleteSpotlight` handler to `ManageModel` that calls `DeleteRound` and redirects with flash message
- [x] 2.2 Add delete button/form for each spotlight round in `Manage.cshtml`

## 3. Tests

- [x] 3.1 Test that deleting a round removes it and its pairings
- [x] 3.2 Test that deleting a non-existent round completes without error
- [x] 3.3 Test that generating a new round after deletion does not consider deleted pairings as history
- [x] 3.4 Test that remaining round numbers are unchanged after deletion (no renumbering)
