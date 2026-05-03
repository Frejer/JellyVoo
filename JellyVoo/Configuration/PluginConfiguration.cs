using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.JellyVoo.Configuration;

/// <summary>
/// Plugin configuration for JellyVoo.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        CountryFilter = string.Empty;
        ChannelCacheMinutes = 15;
    }

    /// <summary>
    /// Gets or sets the country filter (e.g. "Poland", "Turkey"). Leave empty to include all countries.
    /// </summary>
    public string CountryFilter { get; set; }

    /// <summary>
    /// Gets or sets how long (in minutes) the channel list is cached before refreshing.
    /// </summary>
    public int ChannelCacheMinutes { get; set; }
}
