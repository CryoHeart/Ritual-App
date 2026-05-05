using server.Dao.Entities;
using server.Dao.Interfaces;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class AlbumsLogic : IAlbumsLogic
{
    private readonly IAlbumsDao _albumsDao;

    public AlbumsLogic(IAlbumsDao albumsDao)
    {
        _albumsDao = albumsDao;
    }

    public async Task<IReadOnlyCollection<AlbumResponse>> GetAlbumsAsync(string bandId)
    {
        await EnsureBandExistsAsync(bandId);
        var albums = await _albumsDao.GetByBandIdAsync(bandId);
        return albums.Select(MapAlbum).ToList();
    }

    public async Task<IReadOnlyCollection<AlbumWithSongsResponse>> GetAlbumsWithSongsAsync(string bandId)
    {
        await EnsureBandExistsAsync(bandId);
        var albums = await _albumsDao.GetByBandIdWithSongsAsync(bandId);
        return albums.Select(a => new AlbumWithSongsResponse
        {
            AlbumId = a.AlbumId,
            BandId = a.BandId,
            Title = a.Title,
            ReleaseYear = a.ReleaseYear,
            SortOrder = a.SortOrder,
            Songs = a.Songs.Select(s => new AlbumSongResponse
            {
                SongId = s.Id,
                BandId = s.BandId,
                Title = s.Title,
                Bpm = s.Bpm,
                DurationSeconds = s.DurationSeconds,
                AlbumId = s.AlbumId,
                AlbumTitle = a.Title,
                AlbumTrackNumber = s.AlbumTrackNumber,
                Tuning = s.Tuning,
                SongKey = s.SongKey,
                Notes = s.Notes
            }).ToList()
        }).ToList();
    }

    public async Task<AlbumResponse> CreateAlbumAsync(string bandId, CreateAlbumRequest request)
    {
        await EnsureBandExistsAsync(bandId);

        var title = request.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationException("Album title is required.");
        }

        if (title.Length > 150)
        {
            throw new ValidationException("Album title cannot exceed 150 characters.");
        }

        if (request.ReleaseYear.HasValue && (request.ReleaseYear.Value < 1900 || request.ReleaseYear.Value > 2100))
        {
            throw new ValidationException("Release year must be between 1900 and 2100.");
        }

        if (request.SortOrder < 0)
        {
            throw new ValidationException("Sort order must be 0 or greater.");
        }

        var created = await _albumsDao.CreateAsync(bandId, title, request.ReleaseYear, request.SortOrder);
        return MapAlbum(created);
    }

    public async Task<AlbumResponse> UpdateAlbumAsync(string bandId, string albumId, UpdateAlbumRequest request)
    {
        await EnsureBandExistsAsync(bandId);

        var album = await _albumsDao.GetByIdAsync(albumId);
        if (album is null || album.BandId != bandId)
        {
            throw new NotFoundException("Album was not found.");
        }

        var title = request.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationException("Album title is required.");
        }

        if (title.Length > 150)
        {
            throw new ValidationException("Album title cannot exceed 150 characters.");
        }

        if (request.ReleaseYear.HasValue && (request.ReleaseYear.Value < 1900 || request.ReleaseYear.Value > 2100))
        {
            throw new ValidationException("Release year must be between 1900 and 2100.");
        }

        if (request.SortOrder < 0)
        {
            throw new ValidationException("Sort order must be 0 or greater.");
        }

        var updated = await _albumsDao.UpdateAsync(albumId, title, request.ReleaseYear, request.SortOrder);
        return MapAlbum(updated);
    }

    public async Task DeleteAlbumAsync(string bandId, string albumId)
    {
        await EnsureBandExistsAsync(bandId);

        var album = await _albumsDao.GetByIdAsync(albumId);
        if (album is null || album.BandId != bandId)
        {
            throw new NotFoundException("Album was not found.");
        }

        await _albumsDao.DeleteAsync(albumId);
    }

    private async Task EnsureBandExistsAsync(string bandId)
    {
        if (!await _albumsDao.BandExistsAsync(bandId))
        {
            throw new NotFoundException("Band was not found.");
        }
    }

    private static AlbumResponse MapAlbum(AlbumEntity a) => new AlbumResponse
    {
        AlbumId = a.AlbumId ?? string.Empty,
        BandId = a.BandId,
        Title = a.Title,
        ReleaseYear = a.ReleaseYear,
        SortOrder = a.SortOrder,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };
}
