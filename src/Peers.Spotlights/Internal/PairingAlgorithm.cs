namespace Peers.Spotlights.Internal;

internal static class PairingAlgorithm
{
    public static IReadOnlyList<(Guid LeaderId, Guid FollowerId)> GeneratePairings(
        IReadOnlyList<Guid> leaders,
        IReadOnlyList<Guid> followers,
        Dictionary<(Guid LeaderId, Guid FollowerId), int> historicalCounts)
    {
        if (leaders.Count == 0 || followers.Count == 0)
            return [];

        // Build candidate pairs with costs, excluding same-person pairs
        var candidates = new List<(Guid LeaderId, Guid FollowerId, int Cost)>();
        foreach (var leader in leaders)
        {
            foreach (var follower in followers)
            {
                if (leader == follower)
                    continue;

                var count = historicalCounts.GetValueOrDefault((leader, follower));
                candidates.Add((leader, follower, count));
            }
        }

        // Sort by cost ascending, break ties deterministically by DancerId
        candidates.Sort((a, b) =>
        {
            var costCmp = a.Cost.CompareTo(b.Cost);
            if (costCmp != 0) return costCmp;

            var leaderCmp = a.LeaderId.CompareTo(b.LeaderId);
            if (leaderCmp != 0) return leaderCmp;

            return a.FollowerId.CompareTo(b.FollowerId);
        });

        // Greedy assignment
        var usedLeaders = new HashSet<Guid>();
        var usedFollowers = new HashSet<Guid>();
        var result = new List<(Guid LeaderId, Guid FollowerId)>();

        foreach (var (leaderId, followerId, _) in candidates)
        {
            if (usedLeaders.Contains(leaderId) || usedFollowers.Contains(followerId))
                continue;

            result.Add((leaderId, followerId));
            usedLeaders.Add(leaderId);
            usedFollowers.Add(followerId);

            // Stop when we've exhausted the smaller group
            if (result.Count >= Math.Min(leaders.Count, followers.Count))
                break;
        }

        return result;
    }
}
