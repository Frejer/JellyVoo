using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.JellyVoo.Api.Models;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JellyVoo.Api;

/// <summary>
/// HTTP client for the Vavoo.to API, handling channel listing, authentication, and stream URL resolution.
/// </summary>
public class VavooApiClient
{
    private const string ChannelsUrl = "https://vavoo.to/channels";
    private const string ResolveUrl = "https://vavoo.to/mediahubmx-resolve.json";
    private const string LokkeAppPingUrl = "https://www.lokke.app/api/app/ping";
    private const string VavooBox2PingUrl = "https://www.vavoo.tv/api/box/ping2";

    // Static app token from the lokke/vavoo Android app (public, non-user-specific)
    private const string LokkeAppToken =
        "ldCvE092e7gER0rVIajfsXIvRhwlrAzP6_1oEJ4q6HH89QHt24v6NNL_jQJO219hiLOXF2hqEfsUuEWitEIGN4EaHHEHb7Cd7gojc5SQYRFzU3XWo_kMeryAUbcwWnQrnf0-";

    // Static vec parameter for box/ping2 fallback (public, non-user-specific)
    private const string PingVec =
        "9frjpxPjxSNilxJPCJ0XGYs6scej3dW/h/VWlnKUiLSG8IP7mfyDU7NirOlld+VtCKGj03XjetfliDMhIev7wcARo+YTU8KPFuVQP9E2DVXzY2BFo1NhE6qEmPfNDnm74eyl/7iFJ0EETm6XbYyz8IKBkAqPN/Spp3PZ2ulKg3QBSDxcVN4R5zRn7OsgLJ2CNTuWkd/h451lDCp+TtTuvnAEhcQckdsydFhTZCK5IiWrrTIC/d4qDXEd+GtOP4hPdoIuCaNzYfX3lLCwFENC6RZoTBYLrcKVVgbqyQZ7DnLqfLqvf3z0FVUWx9H21liGFpByzdnoxyFkue3NzrFtkRL37xkx9ITucepSYKzUVEfyBh+/3mtzKY26VIRkJFkpf8KVcCRNrTRQn47Wuq4gC7sSwT7eHCAydKSACcUMMdpPSvbvfOmIqeBNA83osX8FPFYUMZsjvYNEE3arbFiGsQlggBKgg1V3oN+5ni3Vjc5InHg/xv476LHDFnNdAJx448ph3DoAiJjr2g4ZTNynfSxdzA68qSuJY8UjyzgDjG0RIMv2h7DlQNjkAXv4k1BrPpfOiOqH67yIarNmkPIwrIV+W9TTV/yRyE1LEgOr4DK8uW2AUtHOPA2gn6P5sgFyi68w55MZBPepddfYTQ+E1N6R/hWnMYPt/i0xSUeMPekX47iucfpFBEv9Uh9zdGiEB+0P3LVMP+q+pbBU4o1NkKyY1V8wH1Wilr0a+q87kEnQ1LWYMMBhaP9yFseGSbYwdeLsX9uR1uPaN+u4woO2g8sw9Y5ze5XMgOVpFCZaut02I5k0U4WPyN5adQjG8sAzxsI3KsV04DEVymj224iqg2Lzz53Xz9yEy+7/85ILQpJ6llCyqpHLFyHq/kJxYPhDUF755WaHJEaFRPxUqbparNX+mCE9Xzy7Q/KTgAPiRS41FHXXv+7XSPp4cy9jli0BVnYf13Xsp28OGs/D8Nl3NgEn3/eUcMN80JRdsOrV62fnBVMBNf36+LbISdvsFAFr0xyuPGmlIETcFyxJkrGZnhHAxwzsvZ+Uwf8lffBfZFPRrNv+tgeeLpatVcHLHZGeTgWWml6tIHwWUqv2TVJeMkAEL5PPS4Gtbscau5HM+FEjtGS+KClfX1CNKvgYJl7mLDEf5ZYQv5kHaoQ6RcPaR6vUNn02zpq5/X3EPIgUKF0r/0ctmoT84B2J1BKfCbctdFY9br7JSJ6DvUxyde68jB+Il6qNcQwTFj4cNErk4x719Y42NoAnnQYC2/qfL/gAhJl8TKMvBt3Bno+va8ve8E0z8yEuMLUqe8OXLce6nCa+L5LYK1aBdb60BYbMeWk1qmG6Nk9OnYLhzDyrd9iHDd7X95OM6X5wiMVZRn5ebw4askTTc50xmrg4eic2U1w1JpSEjdH/u/hXrWKSMWAxaj34uQnMuWxPZEXoVxzGyuUbroXRfkhzpqmqqqOcypjsWPdq5BOUGL/Riwjm6yMI0x9kbO8+VoQ6RYfjAbxNriZ1cQ+AW1fqEgnRWXmjt4Z1M0ygUBi8w71bDML1YG6UHeC2cJ2CCCxSrfycKQhpSdI1QIuwd2eyIpd4LgwrMiY3xNWreAF+qobNxvE7ypKTISNrz0iYIhU0aKNlcGwYd0FXIRfKVBzSBe4MRK2pGLDNO6ytoHxvJweZ8h1XG8RWc4aB5gTnB7Tjiqym4b64lRdj1DPHJnzD4aqRixpXhzYzWVDN2kONCR5i2quYbnVFN4sSfLiKeOwKX4JdmzpYixNZXjLkG14seS6KR0Wl8Itp5IMIWFpnNokjRH76RYRZAcx0jP0V5/GfNNTi5QsEU98en0SiXHQGXnROiHpRUDXTl8FmJORjwXc0AjrEMuQ2FDJDmAIlKUSLhjbIiKw3iaqp5TVyXuz0ZMYBhnqhcwqULqtFSuIKpaW8FgF8QJfP2frADf4kKZG1bQ99MrRrb2A=";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<VavooApiClient> _logger;

    private IReadOnlyList<VavooChannel>? _channelCache;
    private DateTime _channelCacheExpiry = DateTime.MinValue;

    private string? _cachedSignature;
    private DateTime _signatureCacheExpiry = DateTime.MinValue;

    private static readonly TimeSpan SignatureCacheDuration = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Initializes a new instance of the <see cref="VavooApiClient"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="logger">The logger.</param>
    public VavooApiClient(IHttpClientFactory httpClientFactory, ILogger<VavooApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Returns the list of available Vavoo channels, using a timed cache.
    /// </summary>
    /// <param name="cacheMinutes">Cache duration in minutes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of <see cref="VavooChannel"/> objects.</returns>
    public async Task<IReadOnlyList<VavooChannel>> GetChannelsAsync(int cacheMinutes, CancellationToken cancellationToken)
    {
        if (_channelCache != null && DateTime.UtcNow < _channelCacheExpiry)
        {
            return _channelCache;
        }

        _logger.LogInformation("Fetching channel list from {Url}", ChannelsUrl);
        var client = CreateDefaultClient();
        var channels = await client
            .GetFromJsonAsync<List<VavooChannel>>(ChannelsUrl, cancellationToken)
            .ConfigureAwait(false);

        _channelCache = channels ?? new List<VavooChannel>();
        _channelCacheExpiry = DateTime.UtcNow.AddMinutes(cacheMinutes);
        _logger.LogInformation("Fetched {Count} channels from Vavoo.to", _channelCache.Count);
        return _channelCache;
    }

    /// <summary>
    /// Resolves the playback URL for a given channel, attempting authentication via lokke.app first,
    /// then falling back to the ping2 token, and finally to the raw stream URL.
    /// </summary>
    /// <param name="channelId">The integer channel ID from the Vavoo channels list.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A resolved stream URL string.</returns>
    public async Task<string> ResolveStreamUrlAsync(int channelId, CancellationToken cancellationToken)
    {
        var rawUrl = $"https://vavoo.to/play/{channelId}/index.m3u8";

        // Attempt primary path: lokke.app signature + mediahubmx-resolve
        var signature = await GetAuthSignatureAsync(cancellationToken).ConfigureAwait(false);
        if (signature != null)
        {
            var resolved = await ResolveWithSignatureAsync(rawUrl, signature, cancellationToken).ConfigureAwait(false);
            if (resolved != null)
            {
                _logger.LogDebug("Resolved stream for channel {ChannelId} via mediahubmx-resolve", channelId);
                return resolved;
            }
        }

        // Fallback path: ping2 signed token appended as query param
        var ping2Token = await GetPing2TokenAsync(cancellationToken).ConfigureAwait(false);
        if (ping2Token != null)
        {
            var fallbackUrl = $"{rawUrl}?vavoo_auth={Uri.EscapeDataString(ping2Token)}";
            _logger.LogDebug("Using ping2 fallback for channel {ChannelId}", channelId);
            return fallbackUrl;
        }

        // Last resort: return the raw URL and hope the server allows it
        _logger.LogWarning("Could not obtain auth token for channel {ChannelId}; using raw URL", channelId);
        return rawUrl;
    }

    private async Task<string?> GetAuthSignatureAsync(CancellationToken cancellationToken)
    {
        if (_cachedSignature != null && DateTime.UtcNow < _signatureCacheExpiry)
        {
            return _cachedSignature;
        }

        _logger.LogDebug("Requesting auth signature from lokke.app");
        try
        {
            var nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var uniqueId = Guid.NewGuid().ToString("N")[..16];

            var pingBody = new LokkePingRequest
            {
                Token = LokkeAppToken,
                Reason = "app-blur",
                Locale = "en",
                Theme = "dark",
                FirstAppStart = nowMs - 86_400_000,
                LastAppStart = nowMs,
                Metadata = new System.Collections.Generic.Dictionary<string, object>
                {
                    ["device"] = new
                    {
                        type = "Handset",
                        brand = "google",
                        model = "Nexus",
                        name = "21081111RG",
                        uniqueId
                    },
                    ["os"] = new { name = "android", version = "7.1.2", abis = new[] { "arm64-v8a" }, host = "android" },
                    ["app"] = new
                    {
                        platform = "android",
                        version = "1.1.0",
                        buildId = "97215000",
                        engine = "hbc85",
                        signatures = new[] { "6e8a975e3cbf07d5de823a760d4c2547f86c1403105020adee5de67ac510999e" },
                        installer = "com.android.vending"
                    },
                    ["version"] = new { package = "app.lokke.main", binary = "1.1.0", js = "1.1.0" },
                    ["platform"] = new
                    {
                        isAndroid = true, isIOS = false, isTV = false, isWeb = false,
                        isMobile = true, isWebTV = false, isElectron = false
                    }
                },
                Iap = new System.Collections.Generic.Dictionary<string, object> { ["supported"] = true }
            };

            var client = CreateApiClient("okhttp/4.11.0");
            using var response = await client
                .PostAsJsonAsync(LokkeAppPingUrl, pingBody, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("lokke.app ping returned {Status}", response.StatusCode);
                return null;
            }

            var result = await response.Content
                .ReadFromJsonAsync<LokkeAppPingResponse>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (result?.AddonSig is { Length: > 0 } sig)
            {
                _cachedSignature = sig;
                _signatureCacheExpiry = DateTime.UtcNow.Add(SignatureCacheDuration);
                _logger.LogDebug("Obtained auth signature from lokke.app (cached for {Duration})", SignatureCacheDuration);
                return sig;
            }

            _logger.LogWarning("lokke.app ping did not return an addonSig");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to obtain auth signature from lokke.app");
        }

        return null;
    }

    private async Task<string?> ResolveWithSignatureAsync(string url, string signature, CancellationToken cancellationToken)
    {
        try
        {
            var requestBody = new VavooResolveRequest { Url = url };
            var client = CreateApiClient("MediaHubMX/2");
            client.DefaultRequestHeaders.TryAddWithoutValidation("mediahubmx-signature", signature);

            using var response = await client
                .PostAsJsonAsync(ResolveUrl, requestBody, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("mediahubmx-resolve returned {Status}", response.StatusCode);
                return null;
            }

            var raw = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            // The response is either a JSON array of objects with a "url" field,
            // or a single object with a "url" or "data.url" field.
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
            {
                var first = root[0];
                if (first.TryGetProperty("url", out var urlProp) && urlProp.ValueKind == JsonValueKind.String)
                {
                    return urlProp.GetString();
                }
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("url", out var urlProp) && urlProp.ValueKind == JsonValueKind.String)
                {
                    return urlProp.GetString();
                }

                if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object
                    && data.TryGetProperty("url", out var dataUrl) && dataUrl.ValueKind == JsonValueKind.String)
                {
                    return dataUrl.GetString();
                }
            }

            _logger.LogWarning("mediahubmx-resolve response contained no URL: {Response}", raw);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to resolve stream via mediahubmx-resolve");
        }

        return null;
    }

    private async Task<string?> GetPing2TokenAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Requesting fallback ping2 token from vavoo.tv");
        try
        {
            var client = CreateDefaultClient();
            var formContent = new FormUrlEncodedContent(new[] { new System.Collections.Generic.KeyValuePair<string, string>("vec", PingVec) });
            using var response = await client.PostAsync(VavooBox2PingUrl, formContent, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("vavoo.tv ping2 returned {Status}", response.StatusCode);
                return null;
            }

            var result = await response.Content
                .ReadFromJsonAsync<VavooBox2PingResponse>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result?.Response?.Signed;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to obtain ping2 token from vavoo.tv");
            return null;
        }
    }

    private HttpClient CreateDefaultClient()
    {
        var client = _httpClientFactory.CreateClient(nameof(VavooApiClient));
        client.DefaultRequestHeaders.UserAgent.ParseAdd("okhttp/4.11.0");
        return client;
    }

    private HttpClient CreateApiClient(string userAgent)
    {
        var client = _httpClientFactory.CreateClient(nameof(VavooApiClient));
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip");
        return client;
    }
}
