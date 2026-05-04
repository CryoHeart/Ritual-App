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
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };

    public async Task<IReadOnlyCollection<SongEntity>> GetByBandIdAsync(string bandId)
    {
        var rows = await _db.Songs.Where(s => s.BandId == bandId).ToListAsync();
        return rows.Select(Map).ToList();
    }

    public async Task<SongEntity?> GetByIdAsync(string bandId, string songId)
    {
        var r = await _db.Songs.FirstOrDefaultAsync(s => s.BandId == bandId && s.SongId == songId);
        return r == null ? null : Map(r);
    }

    public async Task<SongEntity?> GetByIdAsync(string songId)
    {
        var r = await _db.Songs.FirstOrDefaultAsync(s => s.SongId == songId);
        return r == null ? null : Map(r);
    }

    public async Task<SongEntity> CreateAsync(string bandId, string title, int? bpm, int? durationSeconds)
    {
        var r = new server.Data.Entities.SongEntity
        {
            SongId = Guid.NewGuid().ToString(),
            BandId = bandId,
            Title = title,
            Bpm = bpm,
            DurationSeconds = durationSeconds,
            CreatedAt = DateTime.UtcNow
        };
        _db.Songs.Add(r);
        await _db.SaveChangesAsync();
        return Map(r);
    }
}