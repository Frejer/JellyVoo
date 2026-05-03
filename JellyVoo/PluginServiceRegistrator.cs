using Jellyfin.Plugin.JellyVoo.Api;
using Jellyfin.Plugin.JellyVoo.LiveTv;
using MediaBrowser.Controller;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.JellyVoo;

/// <summary>
/// Registers plugin services with the Jellyfin dependency injection container.
/// </summary>
public class PluginServiceRegistrator : IPluginServiceRegistrator
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
        serviceCollection.AddHttpClient();
        serviceCollection.AddSingleton<VavooApiClient>();
        serviceCollection.AddSingleton<ILiveTvService, VavooLiveTvService>();
    }
}
