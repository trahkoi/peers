using Peers.Training.Sessions;

namespace Peers.Training.Tests.Sessions;

public sealed class SessionServiceSpecificationTests
{
    [Fact]
    public void CreateSession_WithValidName_ReturnsStableSessionId()
    {
        var service = CreateService();

        var sessionId = service.CreateSession("  Tuesday Practice  ");
        service.JoinSession(sessionId, "Alex", SessionRole.Leader);
        var participants = service.ListParticipants(sessionId);

        Assert.NotEqual(Guid.Empty, sessionId);
        Assert.Single(participants);
        Assert.Equal("Alex", participants[0].DancerName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateSession_WithInvalidName_ThrowsValidation(string name)
    {
        var service = CreateService();

        Assert.Throws<SessionValidationException>(() => service.CreateSession(name));
    }

    [Fact]
    public void EndSession_ChangesStateAndBlocksAttendanceMutations()
    {
        var service = CreateService();
        var sessionId = service.CreateSession("Friday");
        service.JoinSession(sessionId, "Mika", SessionRole.Follower);

        service.EndSession(sessionId);

        Assert.Throws<SessionConflictException>(() => service.JoinSession(sessionId, "Robin", SessionRole.Leader));
        Assert.Throws<SessionConflictException>(() => service.LeaveSession(sessionId, "Mika"));
    }

    [Fact]
    public void EndSession_WhenAlreadyEnded_ThrowsConflict()
    {
        var service = CreateService();
        var sessionId = service.CreateSession("Friday");
        service.EndSession(sessionId);

        Assert.Throws<SessionConflictException>(() => service.EndSession(sessionId));
    }

    [Fact]
    public void JoinSession_WithValidInputs_AddsParticipantAndNormalizesRole()
    {
        var service = CreateService();
        var sessionId = service.CreateSession("Friday");

        service.JoinSession(sessionId, "  Alice  ", SessionRole.Leader);
        var participants = service.ListParticipants(sessionId);

        var participant = Assert.Single(participants);
        Assert.Equal("Alice", participant.DancerName);
        Assert.Equal(SessionRole.Leader, participant.Role);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void JoinSession_WithInvalidDancerName_ThrowsValidation(string dancerName)
    {
        var service = CreateService();
        var sessionId = service.CreateSession("Friday");

        Assert.Throws<SessionValidationException>(() => service.JoinSession(sessionId, dancerName, SessionRole.Leader));
    }

    [Fact]
    public void JoinSession_WithInvalidRole_ThrowsValidation()
    {
        var service = CreateService();
        var sessionId = service.CreateSession("Friday");

        Assert.Throws<SessionValidationException>(() => service.JoinSession(sessionId, "Taylor", (SessionRole)999));
    }

    [Fact]
    public void JoinSession_WhenAlreadyParticipant_ThrowsConflictConsistently()
    {
        var service = CreateService();
        var sessionId = service.CreateSession("Friday");
        service.JoinSession(sessionId, "Taylor", SessionRole.Follower);

        Assert.Throws<SessionConflictException>(() => service.JoinSession(sessionId, "Taylor", SessionRole.Follower));
        var participants = service.ListParticipants(sessionId);
        Assert.Single(participants);
    }

    [Fact]
    public void LeaveSession_RemovesParticipant()
    {
        var service = CreateService();
        var sessionId = service.CreateSession("Friday");
        service.JoinSession(sessionId, "Jules", SessionRole.Leader);

        service.LeaveSession(sessionId, "Jules");

        Assert.Empty(service.ListParticipants(sessionId));
    }

    [Fact]
    public void LeaveSession_WhenNotParticipating_ThrowsNotFoundConsistently()
    {
        var service = CreateService();
        var sessionId = service.CreateSession("Friday");

        Assert.Throws<SessionNotFoundException>(() => service.LeaveSession(sessionId, "Unknown"));
        Assert.Throws<SessionNotFoundException>(() => service.LeaveSession(sessionId, "Unknown"));
    }

    [Fact]
    public void UnknownSessionId_ThrowsNotFoundForAllOperations()
    {
        var service = CreateService();
        var missingSessionId = Guid.NewGuid();

        Assert.Throws<SessionNotFoundException>(() => service.EndSession(missingSessionId));
        Assert.Throws<SessionNotFoundException>(() => service.JoinSession(missingSessionId, "Kai", SessionRole.Leader));
        Assert.Throws<SessionNotFoundException>(() => service.LeaveSession(missingSessionId, "Kai"));
        Assert.Throws<SessionNotFoundException>(() => service.ListParticipants(missingSessionId));
    }

    private static ISessionService CreateService() => SessionServiceFactory.CreateInMemory();
}
