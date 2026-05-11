using server.Dao.Entities;
using server.Dao.Interfaces;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class SongsLogic : ISongsLogic
{
    private readonly IAlbumsDao _albumsDao;
    private readonly ISongsDao _songsDao;

    public SongsLogic(IAlbumsDao albumsDao, ISongsDao songsDao)
    {
        _albumsDao = albumsDao;
        _songsDao = songsDao;
    }

    public async Task<IReadOnlyCollection<SongResponse>> GetSongsAsync(string bandId)
    {
        await EnsureBandExistsAsync(bandId);
        var songs = await _songsDao.GetByBandIdAsync(bandId);
        return songs.Select(MapSong).ToList();
    }

    public async Task<SongResponse> GetSongAsync(string bandId, string songId)
    {
        await EnsureBandExistsAsync(bandId);
        var song = await _songsDao.GetByIdAsync(bandId, songId);
        if (song is null)
        {
            throw new NotFoundException("Song was not found.");
        }

        return MapSong(song);
    }

    public async Task<SongResponse> CreateSongAsync(string bandId, CreateSongRequest request)
    {
        await EnsureBandExistsAsync(bandId);

        var title = request.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationException("Song title is required.");
        }

        if (title.Length > 150)
        {
            throw new ValidationException("Song title cannot exceed 150 characters.");
        }

        if (request.Bpm.HasValue && (request.Bpm.Value < 40 || request.Bpm.Value > 300))
        {
            throw new ValidationException("BPM must be between 40 and 300.");
        }

        if (request.DurationSeconds.HasValue && request.DurationSeconds.Value <= 0)
        {
            throw new ValidationException("DurationSeconds must be greater than 0.");
        }

        if (request.AlbumTrackNumber.HasValue && request.AlbumTrackNumber.Value <= 0)
        {
            throw new ValidationException("AlbumTrackNumber must be greater than 0 when provided.");
        }

        if (request.Tuning != null && request.Tuning.Trim().Length > 50)
        {
            throw new ValidationException("Tuning cannot exceed 50 characters.");
        }

        if (request.SongKey != null && request.SongKey.Trim().Length > 50)
        {
            throw new ValidationException("Key cannot exceed 50 characters.");
        }

        var normalizedAlbumId = string.IsNullOrWhiteSpace(request.AlbumId) ? null : request.AlbumId.Trim();
        if (normalizedAlbumId != null)
        {
            var albumExists = await _albumsDao.AlbumExistsForBandAsync(bandId, normalizedAlbumId);
            if (!albumExists)
            {
                throw new ValidationException("Album does not belong to this band.");
            }
        }

        var created = await _songsDao.CreateAsync(
            bandId,
            title,
            normalizedAlbumId,
            request.AlbumTrackNumber,
            request.DurationSeconds,
            request.Bpm,
            request.Tuning?.Trim(),
            request.SongKey?.Trim(),
            request.Notes?.Trim());

        return MapSong(created);
    }

    public async Task<SongResponse> UpdateSongAsync(string bandId, string songId, UpdateSongRequest request)
    {
        await EnsureBandExistsAsync(bandId);

        var song = await _songsDao.GetByIdAsync(bandId, songId);
        if (song is null)
        {
            throw new NotFoundException("Song was not found.");
        }

        var title = request.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationException("Song title is required.");
        }

        if (title.Length > 150)
        {
            throw new ValidationException("Song title cannot exceed 150 characters.");
        }

        if (request.DurationSeconds.HasValue && request.DurationSeconds.Value <= 0)
        {
            throw new ValidationException("DurationSeconds must be greater than 0.");
        }

        if (request.Bpm.HasValue && (request.Bpm.Value < 40 || request.Bpm.Value > 300))
        {
            throw new ValidationException("BPM must be between 40 and 300.");
        }

        if (request.AlbumTrackNumber.HasValue && request.AlbumTrackNumber.Value <= 0)
        {
            throw new ValidationException("AlbumTrackNumber must be greater than 0 when provided.");
        }

        if (request.Tuning != null && request.Tuning.Trim().Length > 50)
        {
            throw new ValidationException("Tuning cannot exceed 50 characters.");
        }

        if (request.SongKey != null && request.SongKey.Trim().Length > 50)
        {
            throw new ValidationException("Key cannot exceed 50 characters.");
        }

        var normalizedAlbumId = string.IsNullOrWhiteSpace(request.AlbumId)
            ? null
            : request.AlbumId.Trim();

        if (normalizedAlbumId != null)
        {
            var albumExists = await _albumsDao.AlbumExistsForBandAsync(bandId, normalizedAlbumId);
            if (!albumExists)
            {
                throw new ValidationException("Album does not belong to this band.");
            }
        }

        var updated = await _songsDao.UpdateAsync(
            songId,
            title,
            normalizedAlbumId,
            request.DurationSeconds,
            request.AlbumTrackNumber,
            request.Bpm,
            request.Tuning?.Trim(),
            request.SongKey?.Trim(),
            request.Notes?.Trim());

        return MapSong(updated);
    }

    public async Task DeleteSongAsync(string bandId, string songId)
    {
        await EnsureBandExistsAsync(bandId);

        var song = await _songsDao.GetByIdAsync(bandId, songId);
        if (song is null)
        {
            throw new NotFoundException("Song was not found.");
        }

        await _songsDao.DeleteAsync(songId);
    }

    private async Task EnsureBandExistsAsync(string bandId)
    {
        if (!await _songsDao.BandExistsAsync(bandId))
        {
            throw new NotFoundException("Band was not found.");
        }
    }

    private static SongResponse MapSong(SongEntity song)
    {
        return new SongResponse
        {
            SongId = song.Id,
            BandId = song.BandId,
            Title = song.Title,
            MusicBrainzRecordingId = song.MusicBrainzRecordingId,
            Bpm = song.Bpm,
            DurationSeconds = song.DurationSeconds,
            AlbumId = song.AlbumId,
            AlbumTitle = song.AlbumTitle,
            AlbumTrackNumber = song.AlbumTrackNumber,
            Tuning = song.Tuning,
            SongKey = song.SongKey,
            Notes = song.Notes,
            CreatedAt = song.CreatedAt,
            UpdatedAt = song.UpdatedAt
        };
    }
}
