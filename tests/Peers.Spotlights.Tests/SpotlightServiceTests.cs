using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Peers.Persistence;
using Peers.Training.Sessions;

namespace Peers.Spotlights.Tests;

public sealed class SpotlightServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ServiceProvider _provider;
    private readonly ISessionService _sessions;
    private readonly ISpotlightService _spotlights;

    public SpotlightServiceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var services = new ServiceCollection();
        services.AddDbContext<PeersDbContext>(options => options.UseSqlite(_connection));
        services.AddTraining();
        services.AddSpotlights();

        _provider = services.BuildServiceProvider();

        var db = _provider.GetRequiredService<PeersDbContext>();
        db.Database.EnsureCreated();

        _sessions = _provider.GetRequiredService<ISessionService>();
        _spotlights = _provider.GetRequiredService<ISpotlightService>();
    }

    private (IReadOnlyList<Guid> Leaders, IReadOnlyList<Guid> Followers) GetDancerIds(Guid sessionId)
    {
        var participants = _sessions.ListParticipants(sessionId);
        var leaders = participants.Where(p => p.Role == SessionRole.Leader).Select(p => p.DancerId).ToList();
        var followers = participants.Where(p => p.Role == SessionRole.Follower).Select(p => p.DancerId).ToList();
        return (leaders, followers);
    }

    [Fact]
    public void GenerateRound_CreatesPairingsFromParticipants()
    {
        var sessionId = _sessions.CreateSession("Test Session");
        _sessions.JoinSession(sessionId, "Maria", SessionRole.Leader);
        _sessions.JoinSession(sessionId, "Tom", SessionRole.Follower);
        _sessions.JoinSession(sessionId, "Alex", SessionRole.Leader);
        _sessions.JoinSession(sessionId, "Sara", SessionRole.Follower);
        var (leaders, followers) = GetDancerIds(sessionId);

        var round = _spotlights.GenerateRound(sessionId, leaders, followers);

        Assert.Equal(sessionId, round.SessionId);
        Assert.Equal(2, round.Pairings.Count);
        Assert.All(round.Pairings, p =>
        {
            Assert.NotEqual(Guid.Empty, p.LeaderDancerId);
            Assert.NotEqual(Guid.Empty, p.FollowerDancerId);
            Assert.NotEqual(p.LeaderDancerId, p.FollowerDancerId);
        });
    }

    [Fact]
    public void GetRound_ReturnsNull_WhenNoRoundExists()
    {
        var sessionId = _sessions.CreateSession("Empty Session");

        var round = _spotlights.GetRound(sessionId);

        Assert.Null(round);
    }

    [Fact]
    public void GetRound_ReturnsPreviouslyGeneratedRound()
    {
        var sessionId = _sessions.CreateSession("Test Session");
        _sessions.JoinSession(sessionId, "Maria", SessionRole.Leader);
        _sessions.JoinSession(sessionId, "Tom", SessionRole.Follower);
        var (leaders, followers) = GetDancerIds(sessionId);

        var generated = _spotlights.GenerateRound(sessionId, leaders, followers);
        var retrieved = _spotlights.GetRound(sessionId);

        Assert.NotNull(retrieved);
        Assert.Equal(generated.Id, retrieved.Id);
        Assert.Equal(generated.Pairings.Count, retrieved.Pairings.Count);
    }

    [Fact]
    public void GenerateRound_ReplacesExistingRound()
    {
        var sessionId = _sessions.CreateSession("Test Session");
        _sessions.JoinSession(sessionId, "Maria", SessionRole.Leader);
        _sessions.JoinSession(sessionId, "Tom", SessionRole.Follower);
        var (leaders, followers) = GetDancerIds(sessionId);

        var first = _spotlights.GenerateRound(sessionId, leaders, followers);
        var second = _spotlights.GenerateRound(sessionId, leaders, followers);

        Assert.NotEqual(first.Id, second.Id);

        var current = _spotlights.GetRound(sessionId);
        Assert.Equal(second.Id, current!.Id);
    }

    [Fact]
    public void GenerateRound_UsesHistoryAcrossSessions()
    {
        // Session 1
        var session1 = _sessions.CreateSession("Session 1");
        _sessions.JoinSession(session1, "Maria", SessionRole.Leader);
        _sessions.JoinSession(session1, "Tom", SessionRole.Follower);
        _sessions.JoinSession(session1, "Alex", SessionRole.Leader);
        _sessions.JoinSession(session1, "Sara", SessionRole.Follower);
        var (leaders1, followers1) = GetDancerIds(session1);
        _spotlights.GenerateRound(session1, leaders1, followers1);

        // Session 2: same dancers — should avoid same pairings
        var session2 = _sessions.CreateSession("Session 2");
        _sessions.JoinSession(session2, "Maria", SessionRole.Leader);
        _sessions.JoinSession(session2, "Tom", SessionRole.Follower);
        _sessions.JoinSession(session2, "Alex", SessionRole.Leader);
        _sessions.JoinSession(session2, "Sara", SessionRole.Follower);
        var (leaders2, followers2) = GetDancerIds(session2);
        var round2 = _spotlights.GenerateRound(session2, leaders2, followers2);

        var round1 = _spotlights.GetRound(session1);
        Assert.NotNull(round1);

        var round1Pairs = round1.Pairings.Select(p => (p.LeaderDancerId, p.FollowerDancerId)).ToHashSet();
        var round2Pairs = round2.Pairings.Select(p => (p.LeaderDancerId, p.FollowerDancerId)).ToHashSet();

        Assert.NotEqual(round1Pairs, round2Pairs);
    }

    [Fact]
    public void GenerateRound_HandlesEmptyLists()
    {
        var sessionId = Guid.NewGuid();

        var round = _spotlights.GenerateRound(sessionId, [], []);

        Assert.Empty(round.Pairings);
    }

    [Fact]
    public void GenerateRound_PairingsHaveSequentialOrder()
    {
        var sessionId = _sessions.CreateSession("Test Session");
        _sessions.JoinSession(sessionId, "Maria", SessionRole.Leader);
        _sessions.JoinSession(sessionId, "Tom", SessionRole.Follower);
        _sessions.JoinSession(sessionId, "Alex", SessionRole.Leader);
        _sessions.JoinSession(sessionId, "Sara", SessionRole.Follower);
        _sessions.JoinSession(sessionId, "Lee", SessionRole.Leader);
        _sessions.JoinSession(sessionId, "Kim", SessionRole.Follower);
        var (leaders, followers) = GetDancerIds(sessionId);

        var round = _spotlights.GenerateRound(sessionId, leaders, followers);

        var orders = round.Pairings.Select(p => p.Order).ToList();
        Assert.Equal([1, 2, 3], orders);
    }

    public void Dispose()
    {
        _provider.Dispose();
        _connection.Dispose();
    }
}
