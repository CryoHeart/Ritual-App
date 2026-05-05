using Microsoft.EntityFrameworkCore;
using server.Dao.Entities;
using server.Dao.Interfaces;
using server.Data;

namespace server.Dao.Implementations;

public class AlbumsDao : IAlbumsDao
{
    private readonly RitualDbContext _db;

    public AlbumsDao(RitualDbContext db)
    {
        _db = db;
    }

    public async Task<bool> AlbumExistsForBandAsync(string bandId, string albumId)
    {
        return await _db.Albums.AnyAsync(a => a.BandId == bandId && a.AlbumId == albumId);
    }

    public async Task<bool> BandExistsAsync(string bandId)
    {
        return await _db.Bands.AnyAsync(b => b.BandId == bandId);
    }

    public async Task<AlbumEntity?> GetByIdAsync(string albumId)
    {
        var r = await _db.Albums.FirstOrDefaultAsync(a => a.AlbumId == albumId);
        if (r == null) return null;
        return new AlbumEntity
        {
            AlbumId = r.AlbumId,
            BandId = r.BandId,
            Title = r.Title,
            ReleaseYear = r.ReleaseYear,
            SortOrder = r.SortOrder,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }

    public async Task<AlbumEntity> CreateAsync(string bandId, string title, int? releaseYear, int sortOrder)
    {
        var r = new server.Data.Entities.AlbumEntity
        {
            AlbumId = Guid.NewGuid().ToString(),
            BandId = bandId,
            Title = title,
            ReleaseYear = releaseYear,
            SortOrder = sortOrder,
            CreatedAt = DateTime.UtcNow
        };
        _db.Albums.Add(r);
        await _db.SaveChangesAsync();
        return new AlbumEntity
        {
            AlbumId = r.AlbumId,
            BandId = r.BandId,
            Title = r.Title,
            ReleaseYear = r.ReleaseYear,
            SortOrder = r.SortOrder,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }

    public async Task<AlbumEntity> UpdateAsync(string albumId, string title, int? releaseYear, int sortOrder)
    {
        var r = await _db.Albums.FirstAsync(a => a.AlbumId == albumId);
        r.Title = title;
        r.ReleaseYear = releaseYear;
        r.SortOrder = sortOrder;
        r.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return new AlbumEntity
        {
            AlbumId = r.AlbumId,
            BandId = r.BandId,
            Title = r.Title,
            ReleaseYear = r.ReleaseYear,
            SortOrder = r.SortOrder,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }

    public async Task DeleteAsync(string albumId)
    {
        var r = await _db.Albums.FirstAsync(a => a.AlbumId == albumId);
        _db.Albums.Remove(r);
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<AlbumEntity>> GetByBandIdAsync(string bandId)
    {
        var rows = await _db.Albums
            .Where(a => a.BandId == bandId)
            .OrderBy(a => a.SortOrder)
            .ThenBy(a => a.Title)
            .ToListAsync();

        return rows.Select(r => new AlbumEntity
        {
            AlbumId = r.AlbumId,
            BandId = r.BandId,
            Title = r.Title,
            ReleaseYear = r.ReleaseYear,
            SortOrder = r.SortOrder,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();
    }

    public async Task<IReadOnlyCollection<AlbumEntity>> GetByBandIdWithSongsAsync(string bandId)
    {
        var albums = await _db.Albums
            .Where(a => a.BandId == bandId)
            .OrderBy(a => a.SortOrder)
            .ThenBy(a => a.Title)
            .ToListAsync();

        var songs = await _db.Songs
            .Where(s => s.BandId == bandId)
            .ToListAsync();

        var songsByAlbum = songs
            .Where(s => s.AlbumId != null)
            .GroupBy(s => s.AlbumId!)
            .ToDictionary(g => g.Key, g => g
                .OrderBy(s => s.AlbumTrackNumber ?? int.MaxValue)
                .ThenBy(s => s.Title)
                .Select(s => new SongEntity
                {
                    Id = s.SongId,
                    BandId = s.BandId,
                    Title = s.Title,
                    Bpm = s.Bpm,
                    DurationSeconds = s.DurationSeconds,
                    Tuning = s.Tuning,
                    SongKey = s.SongKey,
                    Notes = s.Notes,
                    AlbumId = s.AlbumId,
                    AlbumTrackNumber = s.AlbumTrackNumber,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                }).ToList());

        var albumEntities = albums.Select(r => new AlbumEntity
        {
            AlbumId = r.AlbumId,
            BandId = r.BandId,
            Title = r.Title,
            ReleaseYear = r.ReleaseYear,
            SortOrder = r.SortOrder,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            Songs = songsByAlbum.TryGetValue(r.AlbumId, out var s) ? s : new List<SongEntity>()
        }).ToList();

        var unassignedSongs = songs
            .Where(s => s.AlbumId == null)
            .OrderBy(s => s.Title)
            .Select(s => new SongEntity
            {
                Id = s.SongId,
                BandId = s.BandId,
                Title = s.Title,
                Bpm = s.Bpm,
                DurationSeconds = s.DurationSeconds,
                Tuning = s.Tuning,
                SongKey = s.SongKey,
                Notes = s.Notes,
                AlbumId = null,
                AlbumTrackNumber = null,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

        if (unassignedSongs.Count > 0)
        {
            albumEntities.Add(new AlbumEntity
            {
                AlbumId = null,
                BandId = bandId,
                Title = "Unassigned",
                ReleaseYear = null,
                SortOrder = 999999,
                Songs = unassignedSongs
            });
        }

        return albumEntities;
    }
}
