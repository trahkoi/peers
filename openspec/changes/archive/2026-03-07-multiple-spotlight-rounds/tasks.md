## 1. Data Model

- [x] 1.1 Add `RoundNumber` property to `SpotlightRoundEntity`
- [x] 1.2 Add `RoundNumber` property to `SpotlightRound` DTO record
- [x] 1.3 Create EF Core migration adding `RoundNumber` column with default value 1

## 2. Service API

- [x] 2.1 Replace `GetRound(Guid sessionId)` with `GetRounds(Guid sessionId)` returning `IReadOnlyList<SpotlightRound>` on `ISpotlightService`
- [x] 2.2 Update `SpotlightService.GenerateRound` to append (remove delete-on-generate logic) and compute `RoundNumber` as max + 1
- [x] 2.3 Implement `GetRounds` in `SpotlightService` — return all rounds ordered by `RoundNumber`
- [x] 2.4 Update `ToDto` to include `RoundNumber`

## 3. Web Layer

- [x] 3.1 Update `ManageModel` to use `GetRounds` — change `SpotlightRound?` property to `IReadOnlyList<SpotlightRound>`
- [x] 3.2 Update `Manage.cshtml` to display all rounds with round numbers

## 4. Tests

- [x] 4.1 Update existing `SpotlightServiceTests` for the new `GetRounds` API
- [x] 4.2 Add test: generating multiple rounds appends without deleting previous
- [x] 4.3 Add test: round numbers increment sequentially within a session
- [x] 4.4 Add test: rounds from different sessions are independent
