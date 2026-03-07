using Microsoft.Extensions.DependencyInjection;
using Peers.Persistence;
using Peers.Spotlights.Internal;

namespace Peers.Spotlights;

public static class SpotlightsServiceCollectionExtensions
{
    public static IServiceCollection AddSpotlights(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IModuleEntityConfiguration, SpotlightsEntityConfiguration>();
        services.AddScoped<ISpotlightService, SpotlightService>();

        return services;
    }
}
