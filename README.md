# JellyVoo

A Jellyfin plugin that adds Live TV channels from [Vavoo.to](https://vavoo.to), similar to [TvVoo for Stremio](https://github.com/nicaio/TvVoo) or the Vavoo enigma2 plugin.

## Features

- Live TV integration for Jellyfin via Vavoo.to
- Hundreds of channels from multiple countries (Turkey, Poland, Germany, Italy, etc.)
- Optional country filter to show only channels from selected countries
- Configurable channel list cache
- Automatic stream URL resolution with authentication (lokke.app / mediahubmx)
- Fallback authentication via vavoo.tv ping2

## Requirements

- Jellyfin 10.9.x
- .NET 8 runtime (included with Jellyfin)

## Installation

### Manual

1. Download the latest release ZIP from the [Releases](../../releases) page.
2. Extract `Jellyfin.Plugin.JellyVoo.dll` into the Jellyfin plugins directory:
   - Linux: `~/.local/share/jellyfin/plugins/`
   - Windows: `%APPDATA%\Jellyfin\plugins\`
   - Docker: `/config/data/plugins/`
3. Restart Jellyfin.

### Plugin Repository (Manifest)

Add the following URL to **Dashboard → Plugins → Repositories**:

```
https://raw.githubusercontent.com/Frejer/JellyVoo/main/manifest.json
```

## Configuration

1. In Jellyfin, go to **Dashboard → Plugins → JellyVoo**.
2. Optionally enter a comma-separated list of countries to filter channels (e.g. `Poland, Germany`). Leave empty to show all countries.
3. Adjust the channel cache duration if needed (default: 15 minutes).
4. Click **Save**.
5. Go to **Dashboard → Live TV** and verify that channels are listed under the JellyVoo tuner.

## Building from source

```bash
dotnet build JellyVoo/JellyVoo.csproj -c Release
```

The output DLL will be in `JellyVoo/bin/Release/net8.0/Jellyfin.Plugin.JellyVoo.dll`.

## License

MIT
