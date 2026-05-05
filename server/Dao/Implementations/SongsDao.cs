using Microsoft.EntityFrameworkCore;
using server.Dao.Entities;
using server.Dao.Interfaces;
using server.Data;

namespace server.Dao.Implementations;

public class SongsDao : ISongsDao
{
    private readonly RitualDbContext _db;

    public SongsDao(RitualDbContext db)
    {
        _db = db;
    }

    private static SongEntity Map(server.Data.Entities.SongEntity r) => new SongEntity
    {
        Id = r.SongId,
        BandId = r.BandId,
        Title = r.Title,
        Bpm = r.Bpm,
        DurationSeconds = r.DurationSeconds,
        Tuning = r.Tuning,
        SongKey = r.SongKey,
        Notes = r.Notes,
        AlbumId = r.AlbumId,
        AlbumTrackNumber = r.AlbumTrackNumber,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };

    private static SongEntity Map(server.Data.Entities.SongEntity r, string? albumTitle) => new SongEntity
    {
        Id = r.SongId,
        BandId = r.BandId,
        Title = r.Title,
        Bpm = r.Bpm,
        DurationSeconds = r.DurationSeconds,
        Tuning = r.Tuning,
        SongKey = r.SongKey,
        Notes = r.Notes,
        AlbumId = r.AlbumId,
        AlbumTitle = albumTitle,
        AlbumTrackNumber = r.AlbumTrackNumber,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };

    public async Task<bool> BandExistsAsync(string bandId)
    {
        return await _db.Bands.AnyAsync(b => b.BandId == bandId);
    }

    public async Task<IReadOnlyCollection<SongEntity>> GetByBandIdAsync(string bandId)
    {
        var rows = await (
            from s in _db.Songs
            join a in _db.Albums on s.AlbumId equals a.AlbumId into albumJoin
            from a in albumJoin.DefaultIfEmpty()
            where s.BandId == bandId
            orderby s.Title
            select new { Song = s, AlbumTitle = a != null ? a.Title : null }
        ).ToListAsync();

        return rows.Select(r => Map(r.Song, r.AlbumTitle)).ToList();
    }

    public async Task<SongEntity?> GetByIdAsync(string bandId, string songId)
    {
        var row = await (
            from s in _db.Songs
            join a in _db.Albums on s.AlbumId equals a.AlbumId into albumJoin
            from a in albumJoin.DefaultIfEmpty()
            where s.BandId == bandId && s.SongId == songId
            select new { Song = s, AlbumTitle = a != null ? a.Title : null }
        ).FirstOrDefaultAsync();

        return row == null ? null : Map(row.Song, row.AlbumTitle);
    }

    public async Task<SongEntity?> GetByIdAsync(string songId)
    {
        var r = await _db.Songs.FirstOrDefaultAsync(s => s.SongId == songId);
        return r == null ? null : Map(r);
    }

    public async Task<SongEntity> CreateAsync(
        string bandId,
        string title,
        string? albumId,
        int? albumTrackNumber,
        int? durationSeconds,
        int? bpm,
        string? tuning,
        string? songKey,
        string? notes)
    {
        var r = new server.Data.Entities.SongEntity
        {
            SongId = Guid.NewGuid().ToString(),
            BandId = bandId,
            Title = title,
            AlbumId = albumId,
            AlbumTrackNumber = albumTrackNumber,
            DurationSeconds = durationSeconds,
            Bpm = bpm,
            Tuning = tuning,
            SongKey = songKey,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
        _db.Songs.Add(r);
        await _db.SaveChangesAsync();

        var albumTitle = r.AlbumId == null
            ? null
            : await _db.Albums
                .Where(a => a.AlbumId == r.AlbumId)
                .Select(a => a.Title)
                .FirstOrDefaultAsync();

        return Map(r, albumTitle);
    }

    public async Task<SongEntity> UpdateAsync(
        string songId,
        string title,
        string? albumId,
        int? durationSeconds,
        int? albumTrackNumber,
        int? bpm,
        string? tuning,
        string? songKey,
        string? notes)
    {
        var song = await _db.Songs.FirstAsync(s => s.SongId == songId);

        song.Title = title;
        song.AlbumId = albumId;
        song.DurationSeconds = durationSeconds;
        song.AlbumTrackNumber = albumTrackNumber;
        song.Bpm = bpm;
        song.Tuning = tuning;
        song.SongKey = songKey;
        song.Notes = notes;
        song.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        var albumTitle = song.AlbumId == null
            ? null
            : await _db.Albums
                .Where(a => a.AlbumId == song.AlbumId)
                .Select(a => a.Title)
                .FirstOrDefaultAsync();

        return Map(song, albumTitle);
    }

    public async Task DeleteAsync(string songId)
    {
        var song = await _db.Songs.FirstAsync(s => s.SongId == songId);
        _db.Songs.Remove(song);
        await _db.SaveChangesAsync();
    }
}