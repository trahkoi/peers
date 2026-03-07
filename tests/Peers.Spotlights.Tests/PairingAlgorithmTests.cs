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
    public void UnbalancedGroups_MoreLeaders_SomeLeadersSitOut()
    {
        var leaders = new[] { Id(1), Id(2), Id(3) };
        var followers = new[] { Id(4) };

        var result = PairingAlgorithm.GeneratePairings(leaders, followers, []);

        Assert.Single(result);
        Assert.Contains(result[0].FollowerId, followers);
    }

    [Fact]
    public void UnbalancedGroups_MoreFollowers_SomeFollowersSitOut()
    {
        var leaders = new[] { Id(1) };
        var followers = new[] { Id(2), Id(3), Id(4) };

        var result = PairingAlgorithm.GeneratePairings(leaders, followers, []);

        Assert.Single(result);
        Assert.Contains(result[0].LeaderId, leaders);
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
}
