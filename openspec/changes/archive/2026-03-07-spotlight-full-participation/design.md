## Context

The `PairingAlgorithm.GeneratePairings` method performs a single greedy pass that stops after `min(leaders, followers)` pairs. With 5 leaders and 2 followers, only 2 pairings are created and 3 leaders never dance.

The algorithm already uses cost-based sorting (historical pairing counts) to minimize repeat pairings. The challenge is extending this to multiple sub-rounds while keeping the cost optimization.

## Goals / Non-Goals

**Goals:**
- Every dancer appears in at least one pairing per round
- Minority-side dancers repeat across sub-rounds to fill gaps
- Historical cost optimization still applies across all sub-rounds
- Deterministic output (same inputs produce same results)

**Non-Goals:**
- Balancing how many times minority-side dancers repeat (fair distribution of repeats is nice-to-have but not required)
- Changing the data model or API surface

## Decisions

### 1. Iterative sub-round generation within the same algorithm

After the initial greedy pass, check if any dancers from the larger group are unpaired. If so, run additional greedy passes using only the unpaired dancers from the larger group and all dancers from the smaller group (resetting their "used" state).

This keeps the algorithm in a single method and avoids introducing a sub-round abstraction in the data model.

**Alternative considered**: Generating all sub-rounds upfront by computing `ceil(max/min)` rounds — rejected because it overcomplicates the algorithm and may create unnecessary pairings beyond ensuring full participation.

### 2. Accumulate historical costs across sub-rounds

When generating sub-round N, treat pairings from sub-rounds 1..N-1 as additional history (cost +1 each). This ensures minority-side dancers rotate partners across sub-rounds rather than always pairing with the same person.

### 3. No data model changes

Pairings from all sub-rounds are flattened into a single list with sequential `Order` values. The `SpotlightRound` already holds a `List<Pairing>` — the consumer doesn't need to know about sub-rounds.

## Risks / Trade-offs

- **[Minority-side dancers dance more]** In a 5-leader, 2-follower scenario, each follower dances 2-3 times while each leader dances once. This is inherent to the "everyone dances" requirement and acceptable for training.
- **[Self-pairing edge case]** If a dancer appears in both leader and follower lists and is the only one in the minority group, they could block pairing entirely. Current self-pair exclusion handles this. If all remaining candidates are self-pairs, the loop terminates naturally.
