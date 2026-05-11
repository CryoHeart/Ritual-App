using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Data.Entities;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class MusicBrainzLogic : IMusicBrainzLogic
{
    private readonly IMusicBrainzClient _musicBrainzClient;
    private readonly RitualDbContext _db;

    public MusicBrainzLogic(IMusicBrainzClient musicBrainzClient, RitualDbContext db)
    {
        _musicBrainzClient = musicBrainzClient;
        _db = db;
    }

    public async Task<IReadOnlyCollection<MusicBrainzArtistResponse>> SearchArtistsAsync(string query)
    {
        var sanitized = ValidateSearchQuery(query);
        return await _musicBrainzClient.SearchArtistsAsync(sanitized);
    }

    public async Task<IReadOnlyCollection<MusicBrainzReleaseGroupResponse>> SearchReleaseGroupsAsync(string query)
    {
        var sanitized = ValidateSearchQuery(query);
        return await _musicBrainzClient.SearchReleaseGroupsAsync(sanitized);
    }

    public async Task<IReadOnlyCollection<MusicBrainzRecordingResponse>> SearchRecordingsAsync(string query)
    {
        var sanitized = ValidateSearchQuery(query);
        return await _musicBrainzClient.SearchRecordingsAsync(sanitized);
    }

    public async Task<IReadOnlyCollection<MusicBrainzReleaseGroupResponse>> GetArtistAlbumsAsync(string artistMbid)
    {
        var normalized = RequireValue(artistMbid, "Artist MBID is required.");
        return await _musicBrainzClient.GetArtistAlbumsAsync(normalized);
    }

    public async Task<IReadOnlyCollection<MusicBrainzReleaseResponse>> GetReleaseGroupReleasesAsync(string releaseGroupMbid)
    {
        var normalized = RequireValue(releaseGroupMbid, "Release group MBID is required.");
        return await _musicBrainzClient.GetReleaseGroupReleasesAsync(normalized);
    }

    public async Task<IReadOnlyCollection<MusicBrainzTrackResponse>> GetReleaseTracksAsync(string releaseMbid)
    {
        var normalized = RequireValue(releaseMbid, "Release MBID is required.");
        return await _musicBrainzClient.GetReleaseTracksAsync(normalized);
    }

    public async Task<MusicBrainzImportSummaryResponse> ImportSelectionAsync(ImportMusicBrainzSelectionRequest request)
    {
        if (request is null)
        {
            throw new ValidationException("Request body is required.");
        }

        var userId = RequireValue(request.UserId, "User id is required.");
        var bandId = RequireValue(request.BandId, "Band id is required.");

        var hasBandAccess = await _db.BandMembers.AnyAsync(m => m.BandId == bandId && m.UserId == userId);
        if (!hasBandAccess)
        {
            throw new ValidationException("You do not have access to import into this band.");
        }

        var band = await _db.Bands.FirstOrDefaultAsync(b => b.BandId == bandId);
        if (band is null)
        {
            throw new NotFoundException("Band was not found.");
        }

        var summary = new MusicBrainzImportSummaryResponse();

        var selectedArtistId = request.Artist?.Id?.Trim();
        var selectedArtistCountry = request.Artist?.Country?.Trim();
        if (!string.IsNullOrWhiteSpace(selectedArtistId))
        {
            if (string.IsNullOrWhiteSpace(band.MusicBrainzArtistId))
            {
                band.MusicBrainzArtistId = selectedArtistId;
                summary.ImportedBands = 1;
            }

            if (string.IsNullOrWhiteSpace(band.Country) && !string.IsNullOrWhiteSpace(selectedArtistCountry))
            {
                band.Country = selectedArtistCountry;
            }

            band.UpdatedAt = DateTime.UtcNow;
        }

        var baseSortOrder = await _db.Albums
            .Where(a => a.BandId == bandId)
            .Select(a => (int?)a.SortOrder)
            .MaxAsync() ?? 0;

        foreach (var albumSelection in request.Albums ?? Array.Empty<ImportedAlbumDto>())
        {
            if (albumSelection is null)
            {
                continue;
            }

            var albumTitle = albumSelection.Title?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(albumTitle))
            {
                continue;
            }

            var releaseGroupId = albumSelection.MusicBrainzReleaseGroupId?.Trim();
            var releaseId = albumSelection.MusicBrainzReleaseId?.Trim();
            var normalizedAlbumTitle = NormalizeText(albumTitle);

            var album = await FindExistingAlbumAsync(bandId, releaseGroupId, normalizedAlbumTitle);
            if (album is null)
            {
                baseSortOrder += 1;
                album = new AlbumEntity
                {
                    AlbumId = Guid.NewGuid().ToString(),
                    BandId = bandId,
                    Title = albumTitle,
                    ReleaseYear = NormalizeReleaseYear(albumSelection.ReleaseYear),
                    SortOrder = baseSortOrder,
                    MusicBrainzReleaseGroupId = releaseGroupId,
                    MusicBrainzReleaseId = releaseId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null
                };
                _db.Albums.Add(album);
                summary.ImportedAlbums += 1;
            }
            else
            {
                var albumChanged = false;
                if (string.IsNullOrWhiteSpace(album.MusicBrainzReleaseGroupId) && !string.IsNullOrWhiteSpace(releaseGroupId))
                {
                    album.MusicBrainzReleaseGroupId = releaseGroupId;
                    albumChanged = true;
                }

                if (string.IsNullOrWhiteSpace(album.MusicBrainzReleaseId) && !string.IsNullOrWhiteSpace(releaseId))
                {
                    album.MusicBrainzReleaseId = releaseId;
                    albumChanged = true;
                }

                if (albumChanged)
                {
                    album.UpdatedAt = DateTime.UtcNow;
                }
            }

            foreach (var songSelection in albumSelection.Songs ?? Array.Empty<ImportedSongDto>())
            {
                if (songSelection is null)
                {
                    continue;
                }

                var songTitle = songSelection.Title?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(songTitle))
                {
                    continue;
                }

                var recordingId = songSelection.MusicBrainzRecordingId?.Trim();
                var normalizedSongTitle = NormalizeText(songTitle);

                var existingSong = await FindExistingSongAsync(bandId, recordingId, normalizedSongTitle);
                if (existingSong is not null)
                {
                    summary.SkippedDuplicates += 1;
                    continue;
                }

                var song = new SongEntity
                {
                    SongId = Guid.NewGuid().ToString(),
                    BandId = bandId,
                    Title = songTitle,
                    MusicBrainzRecordingId = recordingId,
                    DurationSeconds = NormalizeDuration(songSelection.DurationSeconds),
                    AlbumTrackNumber = songSelection.TrackNumber is > 0 ? songSelection.TrackNumber : null,
                    AlbumId = album.AlbumId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null
                };

                _db.Songs.Add(song);
                summary.ImportedSongs += 1;
            }
        }

        await _db.SaveChangesAsync();
        return summary;
    }

    private static string ValidateSearchQuery(string query)
    {
        var sanitized = query?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            throw new ValidationException("Search query is required.");
        }

        if (sanitized.Length > 120)
        {
            throw new ValidationException("Search query must be 120 characters or fewer.");
        }

        return sanitized;
    }

    private static string RequireValue(string? value, string error)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ValidationException(error);
        }

        return normalized;
    }

    private async Task<AlbumEntity?> FindExistingAlbumAsync(string bandId, string? releaseGroupId, string normalizedAlbumTitle)
    {
        if (!string.IsNullOrWhiteSpace(releaseGroupId))
        {
            var byMbid = await _db.Albums.FirstOrDefaultAsync(a =>
                a.BandId == bandId &&
                a.MusicBrainzReleaseGroupId == releaseGroupId);

            if (byMbid is not null)
            {
                return byMbid;
            }
        }

        var candidates = await _db.Albums
            .Where(a => a.BandId == bandId)
            .ToListAsync();

        return candidates.FirstOrDefault(a => NormalizeText(a.Title) == normalizedAlbumTitle);
    }

    private async Task<SongEntity?> FindExistingSongAsync(string bandId, string? recordingId, string normalizedSongTitle)
    {
        if (!string.IsNullOrWhiteSpace(recordingId))
        {
            var byMbid = await _db.Songs.FirstOrDefaultAsync(s =>
                s.BandId == bandId &&
                s.MusicBrainzRecordingId == recordingId);

            if (byMbid is not null)
            {
                return byMbid;
            }
        }

        var candidates = await _db.Songs
            .Where(s => s.BandId == bandId)
            .ToListAsync();

        return candidates.FirstOrDefault(s => NormalizeText(s.Title) == normalizedSongTitle);
    }

    private static string NormalizeText(string? value)
    {
        var chars = (value ?? string.Empty)
            .Trim()
            .ToLowerInvariant()
            .Where(char.IsLetterOrDigit)
            .ToArray();

        return new string(chars);
    }

    private static int? NormalizeDuration(int? durationSeconds)
    {
        if (!durationSeconds.HasValue || durationSeconds.Value <= 0)
        {
            return null;
        }

        return Math.Min(durationSeconds.Value, 7200);
    }

    private static int? NormalizeReleaseYear(int? releaseYear)
    {
        if (!releaseYear.HasValue)
        {
            return null;
        }

        return releaseYear.Value is >= 1900 and <= 2100 ? releaseYear : null;
    }
}
