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
            var subRound = MinCostMatching(unpairedLeaders, unpairedFollowers, runningCounts);

            if (subRound.Count == 0)
                break;

            result.AddRange(subRound);

            var pairedLeaders = new HashSet<Guid>(subRound.Select(p => p.LeaderId));
            var pairedFollowers = new HashSet<Guid>(subRound.Select(p => p.FollowerId));

            foreach (var (leaderId, followerId) in subRound)
            {
                var key = (leaderId, followerId);
                runningCounts[key] = runningCounts.GetValueOrDefault(key) + 1;
            }

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

    private static List<(Guid LeaderId, Guid FollowerId)> MinCostMatching(
        IReadOnlyList<Guid> leaders,
        IReadOnlyList<Guid> followers,
        Dictionary<(Guid, Guid), int> costs)
    {
        var n = leaders.Count;
        var m = followers.Count;
        var size = Math.Min(n, m);

        // Build cost matrix (rows = leaders, cols = followers)
        // Self-pairs get a very high cost to effectively exclude them
        const int selfPairCost = 1_000_000;
        var costMatrix = new int[n, m];
        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < m; j++)
            {
                if (leaders[i] == followers[j])
                    costMatrix[i, j] = selfPairCost;
                else
                    costMatrix[i, j] = costs.GetValueOrDefault((leaders[i], followers[j]));
            }
        }

        // Use Hungarian algorithm on the smaller dimension
        var assignments = Hungarian(costMatrix, n, m);

        var result = new List<(Guid LeaderId, Guid FollowerId)>();
        foreach (var (row, col) in assignments)
        {
            if (costMatrix[row, col] >= selfPairCost)
                continue;
            result.Add((leaders[row], followers[col]));
        }

        // Sort deterministically by leader GUID then follower GUID
        result.Sort((a, b) =>
        {
            var cmp = a.LeaderId.CompareTo(b.LeaderId);
            return cmp != 0 ? cmp : a.FollowerId.CompareTo(b.FollowerId);
        });

        return result;
    }

    /// <summary>
    /// Hungarian algorithm for min-cost assignment on a rectangular cost matrix.
    /// Returns (row, col) pairs for the optimal matching of size min(rows, cols).
    /// </summary>
    private static List<(int Row, int Col)> Hungarian(int[,] cost, int rows, int cols)
    {
        // Transpose if more rows than columns so we always have rows <= cols
        var transposed = rows > cols;
        if (transposed)
        {
            var t = new int[cols, rows];
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                    t[j, i] = cost[i, j];
            cost = t;
            (rows, cols) = (cols, rows);
        }

        // Standard Hungarian on rows x cols where rows <= cols
        // u[i] = potential for row i (1-indexed, 0 is dummy)
        // v[j] = potential for col j
        var u = new int[rows + 1];
        var v = new int[cols + 1];
        var assignment = new int[cols + 1]; // assignment[j] = row assigned to col j

        for (var i = 1; i <= rows; i++)
        {
            // Find augmenting path from row i
            var links = new int[cols + 1]; // links[j] = previous col in alternating path
            var mins = new int[cols + 1];  // mins[j] = current minimum reduced cost to col j
            var visited = new bool[cols + 1];

            for (var j = 0; j <= cols; j++)
            {
                mins[j] = int.MaxValue;
                links[j] = 0;
            }

            assignment[0] = i;
            var currentCol = 0;

            do
            {
                visited[currentCol] = true;
                var currentRow = assignment[currentCol];
                var delta = int.MaxValue;
                var nextCol = 0;

                for (var j = 1; j <= cols; j++)
                {
                    if (visited[j]) continue;

                    var reducedCost = cost[currentRow - 1, j - 1] - u[currentRow] - v[j];
                    if (reducedCost < mins[j])
                    {
                        mins[j] = reducedCost;
                        links[j] = currentCol;
                    }

                    if (mins[j] < delta)
                    {
                        delta = mins[j];
                        nextCol = j;
                    }
                }

                // Update potentials
                for (var j = 0; j <= cols; j++)
                {
                    if (visited[j])
                    {
                        u[assignment[j]] += delta;
                        v[j] -= delta;
                    }
                    else
                    {
                        mins[j] -= delta;
                    }
                }

                currentCol = nextCol;
            } while (assignment[currentCol] != 0);

            // Trace back augmenting path
            while (currentCol != 0)
            {
                var prevCol = links[currentCol];
                assignment[currentCol] = assignment[prevCol];
                currentCol = prevCol;
            }
        }

        var result = new List<(int, int)>();
        for (var j = 1; j <= cols; j++)
        {
            if (assignment[j] == 0) continue;
            var row = assignment[j] - 1;
            var col = j - 1;
            result.Add(transposed ? (col, row) : (row, col));
        }

        return result;
    }
}
