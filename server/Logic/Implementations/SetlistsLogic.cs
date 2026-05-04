using server.Dao.Entities;
using server.Dao.Interfaces;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class SetlistsLogic : ISetlistsLogic
{
    private readonly IBandsDao _bandsDao;
    private readonly ISongsDao _songsDao;
    private readonly ISetlistsDao _setlistsDao;

    public SetlistsLogic(IBandsDao bandsDao, ISongsDao songsDao, ISetlistsDao setlistsDao)
    {
        _bandsDao = bandsDao;
        _songsDao = songsDao;
        _setlistsDao = setlistsDao;
    }

    public async Task<IReadOnlyCollection<SetlistResponse>> GetSetlistsAsync(string bandId)
    {
        await EnsureBandExistsAsync(bandId);
        var setlists = await _setlistsDao.GetByBandIdAsync(bandId);
        return await Task.WhenAll(setlists.Select(MapSetlistAsync));
    }

    public async Task<SetlistResponse> GetSetlistAsync(string bandId, string setlistId)
    {
        await EnsureBandExistsAsync(bandId);
        var setlist = await GetSetlistForBandAsync(bandId, setlistId);
        return await MapSetlistAsync(setlist);
    }

    public async Task<SetlistResponse> CreateSetlistAsync(string bandId, CreateSetlistRequest request)
    {
        await EnsureBandExistsAsync(bandId);

        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Setlist name is required.");
        }

        if (name.Length > 150)
        {
            throw new ValidationException("Setlist name cannot exceed 150 characters.");
        }

        var created = await _setlistsDao.CreateAsync(bandId, name);
        return await MapSetlistAsync(created);
    }

    public async Task<SetlistResponse> AddSongToSetlistAsync(string bandId, string setlistId, AddSongToSetlistRequest request)
    {
        await EnsureBandExistsAsync(bandId);
        var setlist = await GetSetlistForBandAsync(bandId, setlistId);

        var song = await _songsDao.GetByIdAsync(request.SongId);
        if (song is null)
        {
            throw new NotFoundException("Song was not found.");
        }

        if (song.BandId != setlist.BandId)
        {
            throw new ConflictException("Song must belong to the same band as the setlist.");
        }

        await _setlistsDao.AddSongAsync(setlistId, song.Id);

        var updated = await _setlistsDao.GetByIdAsync(setlistId);
        return await MapSetlistAsync(updated!);
    }

    private async Task EnsureBandExistsAsync(string bandId)
    {
        if (await _bandsDao.GetByIdAsync(bandId) is null)
        {
            throw new NotFoundException("Band was not found.");
        }
    }

    private async Task<SetlistEntity> GetSetlistForBandAsync(string bandId, string setlistId)
    {
        var setlist = await _setlistsDao.GetByIdAsync(setlistId);
        if (setlist is null)
        {
            throw new NotFoundException("Setlist was not found.");
        }

        if (setlist.BandId != bandId)
        {
            throw new ConflictException("Setlist does not belong to the provided band.");
        }

        return setlist;
    }

    private async Task<SetlistResponse> MapSetlistAsync(SetlistEntity setlist)
    {
        var songTasks = setlist.SongIds.Select(async (songId, index) =>
        {
            var song = await _songsDao.GetByIdAsync(songId);
            if (song is null) return null;
            return new SetlistSongResponse
            {
                SongId = song.Id,
                Title = song.Title,
                Bpm = song.Bpm,
                DurationSeconds = song.DurationSeconds,
                Order = index + 1
            };
        });

        var songs = (await Task.WhenAll(songTasks))
            .Where(s => s is not null)
            .Select(s => s!)
            .ToList();

        return new SetlistResponse
        {
            Id = setlist.Id,
            BandId = setlist.BandId,
            Name = setlist.Name,
            Description = setlist.Description,
            Songs = songs
        };
    }
}
