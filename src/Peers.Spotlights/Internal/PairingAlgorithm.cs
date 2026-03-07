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

        var result = new List<(Guid LeaderId, Guid FollowerId)>();
        var runningCounts = new Dictionary<(Guid, Guid), int>(historicalCounts);
        var unpairedLeaders = new List<Guid>(leaders);
        var unpairedFollowers = new List<Guid>(followers);

        while (unpairedLeaders.Count > 0 && unpairedFollowers.Count > 0)
        {
            var subRound = GreedyPass(unpairedLeaders, unpairedFollowers, runningCounts);

            if (subRound.Count == 0)
                break; // No valid pairings possible (e.g., all candidates are self-pairs)

            result.AddRange(subRound);

            // Track which dancers from the larger group are still unpaired
            var pairedLeaders = new HashSet<Guid>(subRound.Select(p => p.LeaderId));
            var pairedFollowers = new HashSet<Guid>(subRound.Select(p => p.FollowerId));

            // Accumulate sub-round pairings as history for subsequent passes
            foreach (var (leaderId, followerId) in subRound)
            {
                var key = (leaderId, followerId);
                runningCounts[key] = runningCounts.GetValueOrDefault(key) + 1;
            }

            // Remove paired dancers from the larger group; reset the smaller group
            if (leaders.Count >= followers.Count)
            {
                unpairedLeaders = unpairedLeaders.Where(id => !pairedLeaders.Contains(id)).ToList();
                unpairedFollowers = new List<Guid>(followers);
            }
            else
            {
                unpairedFollowers = unpairedFollowers.Where(id => !pairedFollowers.Contains(id)).ToList();
                unpairedLeaders = new List<Guid>(leaders);
            }
        }

        return result;
    }

    private static List<(Guid LeaderId, Guid FollowerId)> GreedyPass(
        IReadOnlyList<Guid> leaders,
        IReadOnlyList<Guid> followers,
        Dictionary<(Guid, Guid), int> costs)
    {
        var candidates = new List<(Guid LeaderId, Guid FollowerId, int Cost)>();
        foreach (var leader in leaders)
        {
            foreach (var follower in followers)
            {
                if (leader == follower)
                    continue;

                var cost = costs.GetValueOrDefault((leader, follower));
                candidates.Add((leader, follower, cost));
            }
        }

        candidates.Sort((a, b) =>
        {
            var costCmp = a.Cost.CompareTo(b.Cost);
            if (costCmp != 0) return costCmp;

            var leaderCmp = a.LeaderId.CompareTo(b.LeaderId);
            if (leaderCmp != 0) return leaderCmp;

            return a.FollowerId.CompareTo(b.FollowerId);
        });

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

            if (result.Count >= Math.Min(leaders.Count, followers.Count))
                break;
        }

        return result;
    }
}
