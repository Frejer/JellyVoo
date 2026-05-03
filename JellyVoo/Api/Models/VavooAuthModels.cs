using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.JellyVoo.Api.Models;

/// <summary>
/// Response from the lokke.app ping endpoint used to obtain a Vavoo auth signature.
/// </summary>
public class LokkeAppPingResponse
{
    /// <summary>
    /// Gets or sets the addon signature used to authenticate Vavoo stream resolution requests.
    /// </summary>
    [JsonPropertyName("addonSig")]
    public string? AddonSig { get; set; }
}

/// <summary>
/// Response from the vavoo.tv box/ping2 endpoint (fallback auth).
/// </summary>
public class VavooBox2PingResponse
{
    /// <summary>
    /// Gets or sets the inner response object.
    /// </summary>
    [JsonPropertyName("response")]
    public VavooBox2PingInner? Response { get; set; }
}

/// <summary>
/// Inner response from vavoo.tv box/ping2.
/// </summary>
public class VavooBox2PingInner
{
    /// <summary>
    /// Gets or sets the signed token that can be appended to stream URLs as <c>vavoo_auth</c>.
    /// </summary>
    [JsonPropertyName("signed")]
    public string? Signed { get; set; }
}

/// <summary>
/// Request body sent to <c>vavoo.to/mediahubmx-resolve.json</c>.
/// </summary>
public class VavooResolveRequest
{
    /// <summary>
    /// Gets or sets the BCP-47 language code.
    /// </summary>
    [JsonPropertyName("language")]
    public string Language { get; set; } = "en";

    /// <summary>
    /// Gets or sets the ISO 3166-1 alpha-2 region code.
    /// </summary>
    [JsonPropertyName("region")]
    public string Region { get; set; } = "US";

    /// <summary>
    /// Gets or sets the Vavoo stream URL to resolve.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client version string.
    /// </summary>
    [JsonPropertyName("clientVersion")]
    public string ClientVersion { get; set; } = "3.0.2";
}

/// <summary>
/// A single item in a mediahubmx-resolve response list.
/// </summary>
public class VavooResolveItem
{
    /// <summary>
    /// Gets or sets the resolved stream URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// Ping request body for lokke.app.
/// </summary>
public class LokkePingRequest
{
    /// <summary>Gets or sets the app token.</summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>Gets or sets the trigger reason.</summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = "app-blur";

    /// <summary>Gets or sets the locale.</summary>
    [JsonPropertyName("locale")]
    public string Locale { get; set; } = "en";

    /// <summary>Gets or sets the theme.</summary>
    [JsonPropertyName("theme")]
    public string Theme { get; set; } = "dark";

    /// <summary>Gets or sets device/app metadata.</summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>Gets or sets appFocusTime.</summary>
    [JsonPropertyName("appFocusTime")]
    public int AppFocusTime { get; set; }

    /// <summary>Gets or sets playerActive.</summary>
    [JsonPropertyName("playerActive")]
    public bool PlayerActive { get; set; }

    /// <summary>Gets or sets playDuration.</summary>
    [JsonPropertyName("playDuration")]
    public int PlayDuration { get; set; }

    /// <summary>Gets or sets devMode.</summary>
    [JsonPropertyName("devMode")]
    public bool DevMode { get; set; } = true;

    /// <summary>Gets or sets hasAddon.</summary>
    [JsonPropertyName("hasAddon")]
    public bool HasAddon { get; set; } = true;

    /// <summary>Gets or sets castConnected.</summary>
    [JsonPropertyName("castConnected")]
    public bool CastConnected { get; set; }

    /// <summary>Gets or sets the package name.</summary>
    [JsonPropertyName("package")]
    public string Package { get; set; } = "app.lokke.main";

    /// <summary>Gets or sets the version.</summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.1.0";

    /// <summary>Gets or sets the process.</summary>
    [JsonPropertyName("process")]
    public string Process { get; set; } = "app";

    /// <summary>Gets or sets firstAppStart timestamp.</summary>
    [JsonPropertyName("firstAppStart")]
    public long FirstAppStart { get; set; }

    /// <summary>Gets or sets lastAppStart timestamp.</summary>
    [JsonPropertyName("lastAppStart")]
    public long LastAppStart { get; set; }

    /// <summary>Gets or sets ipLocation.</summary>
    [JsonPropertyName("ipLocation")]
    public object? IpLocation { get; set; }

    /// <summary>Gets or sets adblockEnabled.</summary>
    [JsonPropertyName("adblockEnabled")]
    public bool AdblockEnabled { get; set; }

    /// <summary>Gets or sets iap.</summary>
    [JsonPropertyName("iap")]
    public Dictionary<string, object> Iap { get; set; } = new();
}
