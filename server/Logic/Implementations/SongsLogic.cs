using server.Dao.Entities;
using server.Dao.Interfaces;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class SongsLogic : ISongsLogic
{
    private readonly IBandsDao _bandsDao;
    private readonly ISongsDao _songsDao;

    public SongsLogic(IBandsDao bandsDao, ISongsDao songsDao)
    {
        _bandsDao = bandsDao;
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

        var created = await _songsDao.CreateAsync(bandId, title, request.Bpm, request.DurationSeconds);
        return MapSong(created);
    }

    private async Task EnsureBandExistsAsync(string bandId)
    {
        if (await _bandsDao.GetByIdAsync(bandId) is null)
        {
            throw new NotFoundException("Band was not found.");
        }
    }

    private static SongResponse MapSong(SongEntity song)
    {
        return new SongResponse
        {
            Id = song.Id,
            BandId = song.BandId,
            Title = song.Title,
            Bpm = song.Bpm,
            DurationSeconds = song.DurationSeconds,
            Tuning = song.Tuning,
            SongKey = song.SongKey,
            Notes = song.Notes
        };
    }
}
