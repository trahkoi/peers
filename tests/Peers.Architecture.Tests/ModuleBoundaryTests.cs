using ArchUnitNET.Loader;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Peers.Architecture.Tests;

public sealed class ModuleBoundaryTests
{
    private static readonly System.Reflection.Assembly[] AllAssemblies =
    [
        typeof(Peers.Training.Sessions.ISessionService).Assembly,
        typeof(Peers.Spotlights.ISpotlightService).Assembly,
        typeof(Peers.Persistence.PeersDbContext).Assembly,
    ];

    private static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(AllAssemblies)
        .Build();

    private static readonly string[] SharedAssemblyNames =
    [
        "Peers.Web",
        "Peers.Persistence",
    ];

    private static readonly string[] DomainModuleAssemblyNames = AllAssemblies
        .Select(a => a.GetName().Name!)
        .Where(name => !SharedAssemblyNames.Contains(name))
        .ToArray();

    [Fact]
    public void DomainModules_ShouldNotDependOnEachOther()
    {
        foreach (var module in DomainModuleAssemblyNames)
        {
            var otherModules = DomainModuleAssemblyNames.Where(m => m != module).ToArray();

            foreach (var other in otherModules)
            {
                Types()
                    .That().ResideInAssembly(module)
                    .Should().NotDependOnAny(
                        Types().That().ResideInAssembly(other))
                    .Because($"{module} must not depend on {other}")
                    .WithoutRequiringPositiveResults()
                    .Check(Architecture);
            }
        }
    }
}
