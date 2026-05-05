using server.Dao.Entities;
using server.Dao.Interfaces;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class SetlistsLogic : ISetlistsLogic
{
    private readonly ISongsDao _songsDao;
    private readonly ISetlistsDao _setlistsDao;

    public SetlistsLogic(ISongsDao songsDao, ISetlistsDao setlistsDao)
    {
        _songsDao = songsDao;
        _setlistsDao = setlistsDao;
    }

    public async Task<List<SetlistResponse>> GetSetlistsAsync(string bandId)
    {
        await EnsureBandExistsAsync(bandId);
        var setlists = await _setlistsDao.GetSetlistsByBandIdAsync(bandId);

        var responses = new List<SetlistResponse>(setlists.Count);
        foreach (var setlist in setlists)
        {
            var totalSongs = await _setlistsDao.GetSetlistSongCountAsync(setlist.Id);
            var totalDurationSeconds = await _setlistsDao.GetSetlistTotalDurationSecondsAsync(setlist.Id);

            setlist.TotalSongs = totalSongs;
            setlist.TotalDurationSeconds = totalDurationSeconds;
            responses.Add(MapSetlistSummary(setlist));
        }

        return responses;
    }

    public async Task<SetlistDetailsResponse?> GetSetlistDetailsAsync(string bandId, string setlistId)
    {
        await EnsureBandExistsAsync(bandId);
        var setlist = await GetSetlistWithSongsForBandAsync(bandId, setlistId);
        if (setlist is null)
        {
            return null;
        }

        return MapSetlistDetails(setlist);
    }

    public async Task<SetlistResponse> CreateSetlistAsync(string bandId, CreateSetlistRequest request)
    {
        await EnsureBandExistsAsync(bandId);

        if (request is null)
        {
            throw new ValidationException("Request body is required.");
        }

        var (name, description) = ValidateSetlistPayload(request.Name, request.Description);

        var created = await _setlistsDao.CreateSetlistAsync(new SetlistEntity
        {
            BandId = bandId,
            Name = name,
            Description = description
        });
        return MapSetlistSummary(created);
    }

    public async Task<SetlistResponse?> UpdateSetlistAsync(string bandId, string setlistId, UpdateSetlistRequest request)
    {
        await EnsureBandExistsAsync(bandId);

        if (request is null)
        {
            throw new ValidationException("Request body is required.");
        }

        var existing = await _setlistsDao.GetSetlistByIdAsync(bandId, setlistId);
        if (existing is null)
        {
            return null;
        }

        var (name, description) = ValidateSetlistPayload(request.Name, request.Description);

        var updated = await _setlistsDao.UpdateSetlistAsync(new SetlistEntity
        {
            Id = setlistId,
            BandId = bandId,
            Name = name,
            Description = description
        });

        return updated is null ? null : MapSetlistSummary(updated);
    }

    public async Task<bool> DeleteSetlistAsync(string bandId, string setlistId)
    {
        await EnsureBandExistsAsync(bandId);

        var existing = await _setlistsDao.GetSetlistByIdAsync(bandId, setlistId);
        if (existing is null)
        {
            return false;
        }

        return await _setlistsDao.DeleteSetlistAsync(bandId, setlistId);
    }

    public async Task<SetlistDetailsResponse> AddSongToSetlistAsync(string bandId, string setlistId, AddSongToSetlistRequest request)
    {
        await EnsureBandExistsAsync(bandId);
        var setlist = await GetSetlistForBandAsync(bandId, setlistId);

        var song = await _songsDao.GetByIdAsync(request.SongId);
        if (song is null)
            throw new NotFoundException("Song was not found.");

        if (song.BandId != bandId)
            throw new ConflictException("Song must belong to the same band as the setlist.");

        // Check if song is already in the setlist
        var existing = await _setlistsDao.GetByIdWithSongsAsync(setlistId);
        if (existing?.Songs.Any(ss => ss.SongId == request.SongId) == true)
            throw new ConflictException("Song is already in the setlist.");

        var updated = await _setlistsDao.AddSongAsync(setlistId, song.Id);
        return MapSetlistDetails(updated);
    }

    public async Task<SetlistDetailsResponse> RemoveSongFromSetlistAsync(string bandId, string setlistId, string setlistSongId)
    {
        await EnsureBandExistsAsync(bandId);
        var setlist = await GetSetlistWithSongsForBandAsync(bandId, setlistId);
        if (setlist is null)
            throw new NotFoundException("Setlist was not found.");

        if (!setlist.Songs.Any(ss => ss.SetlistSongId == setlistSongId))
            throw new NotFoundException("Setlist song entry was not found.");

        var updated = await _setlistsDao.RemoveSongAsync(setlistId, setlistSongId);
        return MapSetlistDetails(updated);
    }

    public async Task<SetlistDetailsResponse> ReorderSetlistSongsAsync(string bandId, string setlistId, ReorderSetlistSongsRequest request)
    {
        await EnsureBandExistsAsync(bandId);
        var setlist = await GetSetlistWithSongsForBandAsync(bandId, setlistId);
        if (setlist is null)
            throw new NotFoundException("Setlist was not found.");

        var items = request.Items ?? new List<ReorderSetlistSongsRequest.ReorderItem>();

        // Validate all IDs belong to this setlist
        var validIds = setlist.Songs.Select(ss => ss.SetlistSongId).ToHashSet();
        foreach (var item in items)
        {
            if (!validIds.Contains(item.SetlistSongId))
                throw new ValidationException($"SetlistSongId '{item.SetlistSongId}' does not belong to this setlist.");
        }

        // Validate no duplicate positions
        var positions = items.Select(i => i.PositionIndex).ToList();
        if (positions.Distinct().Count() != positions.Count)
            throw new ValidationException("Duplicate position indexes are not allowed.");

        // Validate 0..n with no gaps
        var sorted = positions.OrderBy(p => p).ToList();
        for (int i = 0; i < sorted.Count; i++)
        {
            if (sorted[i] != i)
                throw new ValidationException("Position indexes must be sequential from 0 with no gaps.");
        }

        var order = items.Select(i => (i.SetlistSongId, i.PositionIndex)).ToList();
        var updated = await _setlistsDao.ReorderSongsAsync(setlistId, order);
        return MapSetlistDetails(updated);
    }

    private async Task EnsureBandExistsAsync(string bandId)
    {
        if (!await _setlistsDao.BandExistsAsync(bandId))
            throw new NotFoundException("Band was not found.");
    }

    private async Task<SetlistEntity> GetSetlistForBandAsync(string bandId, string setlistId)
    {
        var setlist = await _setlistsDao.GetSetlistByIdAsync(bandId, setlistId);
        if (setlist is null)
            throw new NotFoundException("Setlist was not found.");

        return setlist;
    }

    private async Task<SetlistEntity?> GetSetlistWithSongsForBandAsync(string bandId, string setlistId)
    {
        var summary = await _setlistsDao.GetSetlistByIdAsync(bandId, setlistId);
        if (summary is null)
        {
            return null;
        }

        var setlist = await _setlistsDao.GetByIdWithSongsAsync(setlistId);
        if (setlist is null)
            return null;

        return setlist;
    }

    private static (string Name, string? Description) ValidateSetlistPayload(string? nameInput, string? descriptionInput)
    {
        var name = nameInput?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Setlist name is required.");
        }

        if (name.Length > 150)
        {
            throw new ValidationException("Setlist name cannot exceed 150 characters.");
        }

        var description = descriptionInput?.Trim();
        if (description?.Length > 500)
        {
            throw new ValidationException("Setlist description cannot exceed 500 characters.");
        }

        return (name, string.IsNullOrWhiteSpace(description) ? null : description);
    }

    private static SetlistResponse MapSetlistSummary(SetlistEntity setlist) => new SetlistResponse
    {
        SetlistId = setlist.Id,
        BandId = setlist.BandId,
        Name = setlist.Name,
        Description = setlist.Description,
        TotalSongs = setlist.TotalSongs,
        TotalDurationSeconds = setlist.TotalDurationSeconds,
        CreatedAt = setlist.CreatedAt,
        UpdatedAt = setlist.UpdatedAt
    };

    private static SetlistDetailsResponse MapSetlistDetails(SetlistEntity setlist) => new SetlistDetailsResponse
    {
        SetlistId = setlist.Id,
        BandId = setlist.BandId,
        Name = setlist.Name,
        Description = setlist.Description,
        TotalSongs = setlist.TotalSongs,
        TotalDurationSeconds = setlist.TotalDurationSeconds,
        CreatedAt = setlist.CreatedAt,
        UpdatedAt = setlist.UpdatedAt,
        Songs = setlist.Songs.Select(ss => new SetlistSongResponse
        {
            SetlistSongId = ss.SetlistSongId,
            SongId = ss.SongId,
            Title = ss.Title,
            Bpm = ss.Bpm,
            DurationSeconds = ss.DurationSeconds,
            PositionIndex = ss.PositionIndex,
            TransitionNotes = ss.TransitionNotes,
            PerformanceNotes = ss.PerformanceNotes,
            Tuning = ss.Tuning,
            SongKey = ss.SongKey,
            Notes = ss.Notes,
            AlbumId = ss.AlbumId,
            AlbumTitle = ss.AlbumTitle,
            AlbumTrackNumber = ss.AlbumTrackNumber
        }).ToList()
    };
}

