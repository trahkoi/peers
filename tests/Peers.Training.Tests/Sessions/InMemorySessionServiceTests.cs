using Microsoft.Extensions.DependencyInjection;
using Peers.Training.Sessions;

namespace Peers.Training.Tests.Sessions;

public sealed class InMemorySessionServiceTests : SessionServiceSpecificationTests
{
    protected override ISessionService CreateService()
    {
        var services = new ServiceCollection();
        services.AddSessions();
        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ISessionService>();
    }
}
