using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.JellyVoo.Api.Models;

/// <summary>
/// Represents a channel entry returned by the Vavoo.to channels API.
/// </summary>
public class VavooChannel
{
    /// <summary>
    /// Gets or sets the unique channel identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the channel display name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country the channel belongs to.
    /// </summary>
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional channel logo URL.
    /// </summary>
    [JsonPropertyName("logo")]
    public string? Logo { get; set; }

    /// <summary>
    /// Gets or sets the optional channel category / group.
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }
}
