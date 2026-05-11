using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using server.Logic.Interfaces;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class MusicBrainzClient : IMusicBrainzClient
{
    private const string JsonFormat = "json";
    private const string UserAgent = "RitualApp/1.0 contact@example.com";

    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public MusicBrainzClient(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;

        if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
        {
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent);
        }
    }

    public async Task<IReadOnlyCollection<MusicBrainzArtistResponse>> SearchArtistsAsync(string query)
    {
        var doc = await GetJsonAsync("/artist", new Dictionary<string, string>
        {
            ["query"] = $"artist:\"{query}\"",
            ["fmt"] = JsonFormat
        });

        return GetArray(doc.RootElement, "artists")
            .Select(MapArtist)
            .Where(x => !string.IsNullOrWhiteSpace(x.Id) && !string.IsNullOrWhiteSpace(x.Name))
            .ToList();
    }

    public async Task<IReadOnlyCollection<MusicBrainzReleaseGroupResponse>> SearchReleaseGroupsAsync(string query)
    {
        var doc = await GetJsonAsync("/release-group", new Dictionary<string, string>
        {
            ["query"] = $"releasegroup:\"{query}\"",
            ["fmt"] = JsonFormat
        });

        return GetArray(doc.RootElement, "release-groups")
            .Select(MapReleaseGroup)
            .Where(x => !string.IsNullOrWhiteSpace(x.Id) && !string.IsNullOrWhiteSpace(x.Title))
            .ToList();
    }

    public async Task<IReadOnlyCollection<MusicBrainzRecordingResponse>> SearchRecordingsAsync(string query)
    {
        var doc = await GetJsonAsync("/recording", new Dictionary<string, string>
        {
            ["query"] = $"recording:\"{query}\"",
            ["fmt"] = JsonFormat
        });

        return GetArray(doc.RootElement, "recordings")
            .Select(MapRecording)
            .Where(x => !string.IsNullOrWhiteSpace(x.Id) && !string.IsNullOrWhiteSpace(x.Title))
            .ToList();
    }

    public async Task<IReadOnlyCollection<MusicBrainzReleaseGroupResponse>> GetArtistAlbumsAsync(string artistMbid)
    {
        var doc = await GetJsonAsync("/release-group", new Dictionary<string, string>
        {
            ["artist"] = artistMbid,
            ["type"] = "album",
            ["fmt"] = JsonFormat,
            ["limit"] = "100"
        });

        return GetArray(doc.RootElement, "release-groups")
            .Select(MapReleaseGroup)
            .Where(x => !string.IsNullOrWhiteSpace(x.Id) && !string.IsNullOrWhiteSpace(x.Title))
            .ToList();
    }

    public async Task<IReadOnlyCollection<MusicBrainzReleaseResponse>> GetReleaseGroupReleasesAsync(string releaseGroupMbid)
    {
        var doc = await GetJsonAsync("/release", new Dictionary<string, string>
        {
            ["release-group"] = releaseGroupMbid,
            ["fmt"] = JsonFormat,
            ["limit"] = "100"
        });

        return GetArray(doc.RootElement, "releases")
            .Select(MapRelease)
            .Where(x => !string.IsNullOrWhiteSpace(x.Id) && !string.IsNullOrWhiteSpace(x.Title))
            .OrderBy(x => string.IsNullOrWhiteSpace(x.Date) ? 1 : 0)
            .ThenBy(x => x.Date)
            .ThenBy(x => x.Title)
            .ToList();
    }

    public async Task<IReadOnlyCollection<MusicBrainzTrackResponse>> GetReleaseTracksAsync(string releaseMbid)
    {
        var doc = await GetJsonAsync($"/release/{Uri.EscapeDataString(releaseMbid)}", new Dictionary<string, string>
        {
            ["inc"] = "recordings+artists+labels+media",
            ["fmt"] = JsonFormat
        });

        var tracks = new List<MusicBrainzTrackResponse>();
        foreach (var media in GetArray(doc.RootElement, "media"))
        {
            foreach (var track in GetArray(media, "tracks"))
            {
                var recording = GetPropertyOrNull(track, "recording");
                var trackId = GetString(track, "id") ?? GetString(recording, "id") ?? Guid.NewGuid().ToString();
                tracks.Add(new MusicBrainzTrackResponse
                {
                    Id = trackId,
                    RecordingId = GetString(recording, "id"),
                    Title = GetString(track, "title") ?? GetString(recording, "title") ?? "Unknown track",
                    LengthMs = GetInt(track, "length") ?? GetInt(recording, "length"),
                    Position = GetInt(track, "position"),
                    Number = GetString(track, "number"),
                    ArtistCredit = JoinArtistCredit(GetPropertyOrNull(recording, "artist-credit"))
                        ?? JoinArtistCredit(GetPropertyOrNull(track, "artist-credit"))
                });
            }
        }

        return tracks
            .OrderBy(t => t.Position ?? int.MaxValue)
            .ThenBy(t => t.Number)
            .ThenBy(t => t.Title)
            .ToList();
    }

    private async Task<JsonDocument> GetJsonAsync(string path, IReadOnlyDictionary<string, string> queryParams)
    {
        var normalizedPath = (path ?? string.Empty).TrimStart('/');
        var query = string.Join("&", queryParams
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value))
            .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

        var requestPath = string.IsNullOrWhiteSpace(query) ? normalizedPath : $"{normalizedPath}?{query}";
        var cacheKey = $"musicbrainz:{requestPath}";

        if (_cache.TryGetValue<string>(cacheKey, out var cachedJson) && !string.IsNullOrWhiteSpace(cachedJson))
        {
            return JsonDocument.Parse(cachedJson);
        }

        using var response = await _httpClient.GetAsync(requestPath);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync();
        _cache.Set(cacheKey, payload, TimeSpan.FromMinutes(10));

        return JsonDocument.Parse(payload);
    }

    private static MusicBrainzArtistResponse MapArtist(JsonElement artist)
    {
        return new MusicBrainzArtistResponse
        {
            Id = GetString(artist, "id") ?? string.Empty,
            Name = GetString(artist, "name") ?? string.Empty,
            Country = GetString(artist, "country"),
            Disambiguation = GetString(artist, "disambiguation"),
            Type = GetString(artist, "type"),
            Score = GetInt(artist, "score")
        };
    }

    private static MusicBrainzReleaseGroupResponse MapReleaseGroup(JsonElement releaseGroup)
    {
        var artistCredit = GetPropertyOrNull(releaseGroup, "artist-credit");
        return new MusicBrainzReleaseGroupResponse
        {
            Id = GetString(releaseGroup, "id") ?? string.Empty,
            Title = GetString(releaseGroup, "title") ?? string.Empty,
            FirstReleaseDate = GetString(releaseGroup, "first-release-date"),
            PrimaryType = GetString(releaseGroup, "primary-type"),
            SecondaryTypes = GetArray(releaseGroup, "secondary-types")
                .Select(v => v.GetString())
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v!)
                .ToList(),
            ArtistCredit = JoinArtistCredit(artistCredit),
            ArtistId = FirstArtistId(artistCredit),
            Score = GetInt(releaseGroup, "score")
        };
    }

    private static MusicBrainzRecordingResponse MapRecording(JsonElement recording)
    {
        var artistCredit = GetPropertyOrNull(recording, "artist-credit");
        var releaseTitle = GetArray(recording, "releases")
            .Select(r => GetString(r, "title"))
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        return new MusicBrainzRecordingResponse
        {
            Id = GetString(recording, "id") ?? string.Empty,
            Title = GetString(recording, "title") ?? string.Empty,
            LengthMs = GetInt(recording, "length"),
            ArtistCredit = JoinArtistCredit(artistCredit),
            ArtistId = FirstArtistId(artistCredit),
            ReleaseTitle = releaseTitle,
            Score = GetInt(recording, "score")
        };
    }

    private static MusicBrainzReleaseResponse MapRelease(JsonElement release)
    {
        return new MusicBrainzReleaseResponse
        {
            Id = GetString(release, "id") ?? string.Empty,
            Title = GetString(release, "title") ?? string.Empty,
            Date = GetString(release, "date"),
            Country = GetString(release, "country"),
            Status = GetString(release, "status"),
            TrackCount = GetInt(release, "track-count")
        };
    }

    private static JsonElement? GetPropertyOrNull(JsonElement? element, string propertyName)
    {
        if (element is null || element.Value.ValueKind == JsonValueKind.Undefined || element.Value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return element.Value.TryGetProperty(propertyName, out var value) ? value : null;
    }

    private static IReadOnlyCollection<JsonElement> GetArray(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value) || value.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<JsonElement>();
        }

        return value.EnumerateArray().ToArray();
    }

    private static IReadOnlyCollection<JsonElement> GetArray(JsonElement? element, string propertyName)
    {
        if (element is null)
        {
            return Array.Empty<JsonElement>();
        }

        return GetArray(element.Value, propertyName);
    }

    private static string? GetString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value))
        {
            return null;
        }

        return value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString();
    }

    private static string? GetString(JsonElement? element, string propertyName)
    {
        if (element is null)
        {
            return null;
        }

        return GetString(element.Value, propertyName);
    }

    private static int? GetInt(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var intValue))
        {
            return intValue;
        }

        if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static int? GetInt(JsonElement? element, string propertyName)
    {
        if (element is null)
        {
            return null;
        }

        return GetInt(element.Value, propertyName);
    }

    private static string? JoinArtistCredit(JsonElement? artistCreditElement)
    {
        if (artistCreditElement is null || artistCreditElement.Value.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var parts = artistCreditElement.Value
            .EnumerateArray()
            .Select(part => GetString(part, "name") ?? GetString(GetPropertyOrNull(part, "artist"), "name"))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .ToList();

        return parts.Count == 0 ? null : string.Join(", ", parts);
    }

    private static string? FirstArtistId(JsonElement? artistCreditElement)
    {
        if (artistCreditElement is null || artistCreditElement.Value.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (var part in artistCreditElement.Value.EnumerateArray())
        {
            var id = GetString(GetPropertyOrNull(part, "artist"), "id");
            if (!string.IsNullOrWhiteSpace(id))
            {
                return id;
            }
        }

        return null;
    }
}
