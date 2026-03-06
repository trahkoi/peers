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
            .Where(static x => !x.FullName!.Contains(".Migrations."))
            .Select(static x => x.FullName)
            .OrderBy(static x => x, StringComparer.Ordinal)
            .ToArray();

        var expectedExportedTypeNames = new[]
        {
            "Peers.Training.Dancers.Dancer",
            "Peers.Training.Sessions.ISessionService",
            "Peers.Training.Sessions.Participant",
            "Peers.Training.Sessions.SessionConflictException",
            "Peers.Training.Sessions.SessionNotFoundException",
            "Peers.Training.Sessions.SessionRole",
            "Peers.Training.Sessions.SessionServiceCollectionExtensions",
            "Peers.Training.Sessions.SessionSummary",
            "Peers.Training.Sessions.SessionValidationException"
        };

        Assert.Equal(expectedExportedTypeNames, exportedTypeNames);
    }
}
