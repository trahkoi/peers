## Context

With the modular persistence layer in place (`PeersDbContext`, `IModuleEntityConfiguration`, `Dancer` entity), the Spotlights module can be built as an independent project that contributes its own entities and services.

The module generates leader/follower pairings for spotlight rounds, distributing them evenly across all historical sessions.

## Goals / Non-Goals

**Goals:**
- Generate deterministic, fair pairings from session participants
- Track pairing history across sessions
- Module is fully independent from Peers.Training at the code level
- Simple UI for coaches to start a round and step through pairings

**Non-Goals:**
- Scoring or judging
- Audience voting or feedback
- Real-time display / projector mode (future consideration)
- Multiple concurrent rounds per session

## Decisions

### 1. Domain model

**Decision:**

```
SpotlightRound
  Id: Guid
  SessionId: Guid          (opaque reference to Training session)
  CreatedAt: DateTime
  Pairings: List<Pairing>

Pairing
  Id: Guid
  RoundId: Guid
  LeaderDancerId: Guid     (opaque reference to Dancer)
  FollowerDancerId: Guid   (opaque reference to Dancer)
  Order: int               (sequence within the round)
```

**Rationale:** SessionId and DancerId are stored as raw Guids — no navigation properties to Training entities. FK integrity is enforced at the database level via the shared `PeersDbContext`, but the code has no compile-time dependency on Training.

### 2. Pairing algorithm

**Decision:** Deterministic greedy algorithm:

1. Get all leaders and followers present in the session (via `ISessionService`)
2. Build a pairing cost matrix from historical counts:
   ```
               Tom(F)  Sara(F)  Maria(F)
   Maria(L)  [  2  ] [  1   ] [  --   ]
   Alex(L)   [  1  ] [  0   ] [  2   ]
   ```
   (`--` = same person, infinite cost)
3. Generate all possible (leader, follower) pairs, excluding same-person pairs
4. Sort by historical count ascending, break ties deterministically (e.g., by DancerId)
5. Greedily assign: pick the lowest-cost pair, remove both dancers from the pool, repeat
6. If one role has more dancers, some sit out — pick those with the highest recent activity

**Rationale:** Simple, predictable, explainable. The coach can see why pairings were chosen. No randomness means the same inputs always produce the same output (reproducible, testable).

**Trade-off:** Greedy isn't globally optimal — the Hungarian algorithm would find the true minimum-cost assignment. But for typical class sizes (6-20 dancers), the difference is negligible, and greedy is simpler to implement and explain.

### 3. Cross-module communication

**Decision:** Spotlights depends on `ISessionService` from Training to get session participants with their DancerIds and roles. No direct database access to Training tables.

```
Peers.Spotlights ──uses──> ISessionService (from Peers.Training)
                           GetParticipants(sessionId) -> [{DancerId, Role}]
```

**Rationale:** Clean dependency inversion. Spotlights never imports Training internals. The interface is the contract.

**Implication:** `Peers.Spotlights` needs a project reference to `Peers.Training` for `ISessionService`. This is a one-way dependency (Spotlights -> Training), which is acceptable. Training knows nothing about Spotlights.

### 4. One round per session (for now)

**Decision:** A session can have at most one spotlight round. Starting a new round for a session replaces the previous one.

**Rationale:** Simplifies the UI and model. A practice session typically has one spotlight segment. Can be relaxed later if needed.

## Entity Configuration

```csharp
// Peers.Spotlights/Persistence/SpotlightsEntityConfiguration.cs
internal sealed class SpotlightsEntityConfiguration : IModuleEntityConfiguration
{
    public void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SpotlightRoundEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SessionId).IsRequired();
            entity.HasIndex(e => e.SessionId).IsUnique();
        });

        modelBuilder.Entity<PairingEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LeaderDancerId).IsRequired();
            entity.Property(e => e.FollowerDancerId).IsRequired();
            entity.Property(e => e.Order).IsRequired();
            entity.HasOne<SpotlightRoundEntity>()
                  .WithMany(r => r.Pairings)
                  .HasForeignKey(e => e.RoundId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
```

## Risks / Trade-offs

- **One-way dependency on Training** — Spotlights references `ISessionService`. If Training's interface changes, Spotlights must update. Acceptable for a small app with two modules.
- **Greedy vs. optimal pairing** — Greedy may not produce the globally optimal assignment, but the difference is negligible at typical class sizes.
- **Name-based dancer identity** — Inherited from the modular-persistence change. Two different "Maria"s would be treated as one. Coach can manage manually.
- **No sit-out tracking** — If roles are unbalanced, some dancers sit out. The algorithm picks who sits out but doesn't track sit-out history for future fairness. Can be added later.
