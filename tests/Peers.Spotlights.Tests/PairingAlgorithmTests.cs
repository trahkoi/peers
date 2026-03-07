using Peers.Spotlights.Internal;

namespace Peers.Spotlights.Tests;

public sealed class PairingAlgorithmTests
{
    private static Guid Id(int n) => new Guid($"00000000-0000-0000-0000-{n:D12}");

    [Fact]
    public void BalancedGroups_NoHistory_PairsAll()
    {
        var leaders = new[] { Id(1), Id(2) };
        var followers = new[] { Id(3), Id(4) };

        var result = PairingAlgorithm.GeneratePairings(leaders, followers, []);

        Assert.Equal(2, result.Count);
        Assert.All(result, p =>
        {
            Assert.Contains(p.LeaderId, leaders);
            Assert.Contains(p.FollowerId, followers);
        });
    }

    [Fact]
    public void BalancedGroups_WithHistory_AvoidsMostPaired()
    {
        var leaders = new[] { Id(1), Id(2) };
        var followers = new[] { Id(3), Id(4) };
        var history = new Dictionary<(Guid, Guid), int>
        {
            [(Id(1), Id(3))] = 5,
            [(Id(2), Id(4))] = 5,
            [(Id(1), Id(4))] = 0,
            [(Id(2), Id(3))] = 0,
        };

        var result = PairingAlgorithm.GeneratePairings(leaders, followers, history);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.LeaderId == Id(1) && p.FollowerId == Id(4));
        Assert.Contains(result, p => p.LeaderId == Id(2) && p.FollowerId == Id(3));
    }

    [Fact]
    public void UnbalancedGroups_MoreLeaders_AllLeadersPaired()
    {
        var leaders = new[] { Id(1), Id(2), Id(3) };
        var followers = new[] { Id(4) };

        var result = PairingAlgorithm.GeneratePairings(leaders, followers, []);

        Assert.Equal(3, result.Count);
        Assert.All(leaders, l => Assert.Contains(result, p => p.LeaderId == l));
        Assert.All(result, p => Assert.Equal(Id(4), p.FollowerId));
    }

    [Fact]
    public void UnbalancedGroups_MoreFollowers_AllFollowersPaired()
    {
        var leaders = new[] { Id(1) };
        var followers = new[] { Id(2), Id(3), Id(4) };

        var result = PairingAlgorithm.GeneratePairings(leaders, followers, []);

        Assert.Equal(3, result.Count);
        Assert.All(followers, f => Assert.Contains(result, p => p.FollowerId == f));
        Assert.All(result, p => Assert.Equal(Id(1), p.LeaderId));
    }

    [Fact]
    public void SamePerson_ExcludedFromPairing()
    {
        // Dancer Id(1) is both leader and follower
        var leaders = new[] { Id(1) };
        var followers = new[] { Id(1) };

        var result = PairingAlgorithm.GeneratePairings(leaders, followers, []);

        Assert.Empty(result);
    }

    [Fact]
    public void SamePerson_SkippedButOthersPaired()
    {
        var leaders = new[] { Id(1), Id(2) };
        var followers = new[] { Id(1), Id(3) };

        var result = PairingAlgorithm.GeneratePairings(leaders, followers, []);

        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, p => p.LeaderId == p.FollowerId);
    }

    [Fact]
    public void EmptyLeaders_ReturnsEmpty()
    {
        var result = PairingAlgorithm.GeneratePairings([], [Id(1)], []);
        Assert.Empty(result);
    }

    [Fact]
    public void EmptyFollowers_ReturnsEmpty()
    {
        var result = PairingAlgorithm.GeneratePairings([Id(1)], [], []);
        Assert.Empty(result);
    }

    [Fact]
    public void Deterministic_SameInputsSameOutput()
    {
        var leaders = new[] { Id(1), Id(2), Id(3) };
        var followers = new[] { Id(4), Id(5), Id(6) };
        var history = new Dictionary<(Guid, Guid), int>
        {
            [(Id(1), Id(4))] = 2,
            [(Id(2), Id(5))] = 1,
        };

        var result1 = PairingAlgorithm.GeneratePairings(leaders, followers, history);
        var result2 = PairingAlgorithm.GeneratePairings(leaders, followers, history);

        Assert.Equal(result1, result2);
    }

    [Fact]
    public void HeavilyUnbalanced_6Leaders1Follower_AllLeadersPaired()
    {
        var leaders = new[] { Id(1), Id(2), Id(3), Id(4), Id(5), Id(6) };
        var followers = new[] { Id(7) };

        var result = PairingAlgorithm.GeneratePairings(leaders, followers, []);

        Assert.Equal(6, result.Count);
        Assert.All(leaders, l => Assert.Contains(result, p => p.LeaderId == l));
        Assert.All(result, p => Assert.Equal(Id(7), p.FollowerId));
    }

    [Fact]
    public void FollowersRotatePartners_AcrossSubRounds()
    {
        var leaders = new[] { Id(1), Id(2), Id(3), Id(4) };
        var followers = new[] { Id(5), Id(6) };

        var result = PairingAlgorithm.GeneratePairings(leaders, followers, []);

        Assert.Equal(4, result.Count);

        // Sub-round 1: 2 pairings (each follower gets one leader)
        // Sub-round 2: 2 pairings (each follower gets a different leader)
        var followerId5Partners = result.Where(p => p.FollowerId == Id(5)).Select(p => p.LeaderId).ToList();
        var followerId6Partners = result.Where(p => p.FollowerId == Id(6)).Select(p => p.LeaderId).ToList();

        Assert.Equal(2, followerId5Partners.Count);
        Assert.Equal(2, followerId6Partners.Count);
        // Each follower should have distinct partners (different leaders in each sub-round)
        Assert.Equal(followerId5Partners.Distinct().Count(), followerId5Partners.Count);
        Assert.Equal(followerId6Partners.Distinct().Count(), followerId6Partners.Count);
    }

    [Fact]
    public void Deterministic_WithMultipleSubRounds()
    {
        var leaders = new[] { Id(1), Id(2), Id(3), Id(4), Id(5) };
        var followers = new[] { Id(6), Id(7) };
        var history = new Dictionary<(Guid, Guid), int>
        {
            [(Id(1), Id(6))] = 2,
        };

        var result1 = PairingAlgorithm.GeneratePairings(leaders, followers, history);
        var result2 = PairingAlgorithm.GeneratePairings(leaders, followers, history);

        Assert.Equal(result1, result2);
    }

    [Fact]
    public void ThreeByThree_ThreeRounds_NoRepeats()
    {
        var leaders = new[] { Id(1), Id(2), Id(3) };
        var followers = new[] { Id(4), Id(5), Id(6) };
        var history = new Dictionary<(Guid, Guid), int>();

        var allPairings = new List<(Guid, Guid)>();

        for (var round = 0; round < 3; round++)
        {
            var pairings = PairingAlgorithm.GeneratePairings(leaders, followers, history);
            foreach (var (leaderId, followerId) in pairings)
            {
                allPairings.Add((leaderId, followerId));
                var key = (leaderId, followerId);
                history[key] = history.GetValueOrDefault(key) + 1;
            }
        }

        Assert.Equal(9, allPairings.Count);
        Assert.Equal(9, allPairings.Distinct().Count());
    }

    [Fact]
    public void SelfPair_UnbalancedGroups_StillPairsOthers()
    {
        // Id(1) is in both groups, 3 leaders and 2 followers
        var leaders = new[] { Id(1), Id(2), Id(3) };
        var followers = new[] { Id(1), Id(4) };

        var result = PairingAlgorithm.GeneratePairings(leaders, followers, []);

        Assert.DoesNotContain(result, p => p.LeaderId == p.FollowerId);
        // All 3 leaders should be paired
        Assert.All(leaders, l => Assert.Contains(result, p => p.LeaderId == l));
    }
}
