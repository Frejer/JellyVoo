using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.JellyVoo.Api;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.MediaInfo;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JellyVoo.LiveTv;

/// <summary>
/// Implements <see cref="ILiveTvService"/> to provide Live TV channels from Vavoo.to.
/// </summary>
public class VavooLiveTvService : ILiveTvService
{
    private readonly VavooApiClient _apiClient;
    private readonly ILogger<VavooLiveTvService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="VavooLiveTvService"/> class.
    /// </summary>
    /// <param name="apiClient">The Vavoo API client.</param>
    /// <param name="logger">The logger.</param>
    public VavooLiveTvService(VavooApiClient apiClient, ILogger<VavooLiveTvService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public string Name => "JellyVoo";

    /// <inheritdoc />
    public string HomePageUrl => "https://vavoo.to";

    /// <inheritdoc />
    public async Task<IEnumerable<ChannelInfo>> GetChannelsAsync(CancellationToken cancellationToken)
    {
        var config = Plugin.Instance?.Configuration;
        var cacheMinutes = config?.ChannelCacheMinutes ?? 15;
        var countryFilter = config?.CountryFilter ?? string.Empty;

        var vavooChannels = await _apiClient
            .GetChannelsAsync(cacheMinutes, cancellationToken)
            .ConfigureAwait(false);

        IEnumerable<Api.Models.VavooChannel> filtered = vavooChannels;

        if (!string.IsNullOrWhiteSpace(countryFilter))
        {
            var countries = countryFilter
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            filtered = vavooChannels.Where(ch =>
                countries.Any(c => string.Equals(ch.Country, c, StringComparison.OrdinalIgnoreCase)));
        }

        var results = new List<ChannelInfo>();
        var index = 1;
        foreach (var ch in filtered)
        {
            var channelInfo = new ChannelInfo
            {
                Id = ch.Id.ToString(CultureInfo.InvariantCulture),
                Name = ch.Name,
                Number = index.ToString(CultureInfo.InvariantCulture),
                ChannelType = ChannelType.TV,
                ChannelGroup = string.IsNullOrWhiteSpace(ch.Country) ? "Other" : ch.Country
            };

            if (!string.IsNullOrWhiteSpace(ch.Logo))
            {
                channelInfo.ImageUrl = ch.Logo;
                channelInfo.HasImage = true;
            }

            results.Add(channelInfo);
            index++;
        }

        _logger.LogInformation("JellyVoo returning {Count} channels", results.Count);
        return results;
    }

    /// <inheritdoc />
    public async Task<List<MediaSourceInfo>> GetChannelStreamMediaSources(string channelId, CancellationToken cancellationToken)
    {
        var source = await BuildMediaSourceAsync(channelId, cancellationToken).ConfigureAwait(false);
        return new List<MediaSourceInfo> { source };
    }

    /// <inheritdoc />
    public async Task<MediaSourceInfo> GetChannelStream(string channelId, string streamId, CancellationToken cancellationToken)
    {
        return await BuildMediaSourceAsync(channelId, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task CloseLiveStream(string id, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ResetTuner(string id, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IEnumerable<ProgramInfo>> GetProgramsAsync(
        string channelId,
        DateTime startDateUtc,
        DateTime endDateUtc,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Enumerable.Empty<ProgramInfo>());
    }

    /// <inheritdoc />
    public Task<IEnumerable<TimerInfo>> GetTimersAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Enumerable.Empty<TimerInfo>());
    }

    /// <inheritdoc />
    public Task<IEnumerable<SeriesTimerInfo>> GetSeriesTimersAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Enumerable.Empty<SeriesTimerInfo>());
    }

    /// <inheritdoc />
    public Task<SeriesTimerInfo> GetNewTimerDefaultsAsync(CancellationToken cancellationToken, ProgramInfo? program = null)
    {
        return Task.FromResult(new SeriesTimerInfo { RecordAnyChannel = false, RecordAnyTime = false });
    }

    /// <inheritdoc />
    public Task CancelTimerAsync(string timerId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("JellyVoo does not support recordings.");
    }

    /// <inheritdoc />
    public Task CancelSeriesTimerAsync(string timerId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("JellyVoo does not support recordings.");
    }

    /// <inheritdoc />
    public Task CreateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("JellyVoo does not support recordings.");
    }

    /// <inheritdoc />
    public Task CreateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("JellyVoo does not support recordings.");
    }

    /// <inheritdoc />
    public Task UpdateTimerAsync(TimerInfo updatedTimer, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("JellyVoo does not support recordings.");
    }

    /// <inheritdoc />
    public Task UpdateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("JellyVoo does not support recordings.");
    }

    private async Task<MediaSourceInfo> BuildMediaSourceAsync(string channelId, CancellationToken cancellationToken)
    {
        if (!int.TryParse(channelId, out var id))
        {
            throw new ArgumentException($"Invalid channel ID: {channelId}", nameof(channelId));
        }

        var streamUrl = await _apiClient
            .ResolveStreamUrlAsync(id, cancellationToken)
            .ConfigureAwait(false);

        _logger.LogDebug("Resolved stream URL for channel {ChannelId}: {Url}", channelId, streamUrl);

        return new MediaSourceInfo
        {
            Id = channelId,
            Path = streamUrl,
            Protocol = MediaProtocol.Http,
            MediaStreams = new List<MediaStream>(),
            IsRemote = true,
            SupportsDirectPlay = true,
            SupportsDirectStream = true,
            IsInfiniteStream = true
        };
    }
}
