using Peers.Training.Sessions;

namespace Peers.Training.Tests.Sessions;

public sealed class SessionApiBoundaryTests
{
    [Fact]
    public void SessionAssembly_ExportsOnlyIntendedPublicSessionApi()
    {
        var assembly = typeof(ISessionService).Assembly;
        var exportedTypeNames = assembly
            .GetExportedTypes()
            .Select(static x => x.FullName)
            .OrderBy(static x => x, StringComparer.Ordinal)
            .ToArray();

        var expectedExportedTypeNames = new[]
        {
            "Peers.Training.Sessions.ISessionService",
            "Peers.Training.Sessions.Participant",
            "Peers.Training.Sessions.SessionConflictException",
            "Peers.Training.Sessions.SessionNotFoundException",
            "Peers.Training.Sessions.SessionRole",
            "Peers.Training.Sessions.SessionServiceCollectionExtensions",
            "Peers.Training.Sessions.SessionValidationException"
        };

        Assert.Equal(expectedExportedTypeNames, exportedTypeNames);
    }
}
