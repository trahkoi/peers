## Why

The spotlight pairing algorithm currently creates `min(leaders, followers)` pairings per round. When groups are unbalanced (e.g., 3 leaders and 1 follower), only one pairing is created and the remaining dancers sit out entirely. In a training context, every participant should get to dance at least once per round.

## What Changes

- Modify the pairing algorithm to generate multiple sub-rounds when groups are unbalanced, rotating participants from the larger group so that every dancer is paired at least once
- A round's pairings list may now contain a dancer more than once (the minority-side dancers repeat across sub-rounds to fill gaps)
- The `Pairing.Order` field naturally separates sub-rounds (e.g., sub-round 1 has orders 1-2, sub-round 2 has orders 3-4)

## Capabilities

### New Capabilities
- `spotlight-full-participation`: Ensure all participants in a spotlight round get paired at least once, generating multiple sub-rounds if needed

### Modified Capabilities
_(none — no existing spotlight spec exists)_

## Impact

- **Code**: `PairingAlgorithm.GeneratePairings` in `Peers.Spotlights` — core algorithm change
- **Tests**: Existing unbalanced-group tests need updating to expect full participation instead of sit-outs
- **Data model**: No schema changes — pairings are still `(LeaderDancerId, FollowerDancerId, Order)`. A dancer may now appear in multiple pairings within a round.
- **API**: `ISpotlightService` interface unchanged
