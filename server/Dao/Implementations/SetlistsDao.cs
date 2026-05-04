using Microsoft.EntityFrameworkCore;
using server.Dao.Entities;
using server.Dao.Interfaces;
using server.Data;

namespace server.Dao.Implementations;

public class SetlistsDao : ISetlistsDao
{
    private readonly RitualDbContext _db;

    public SetlistsDao(RitualDbContext db)
    {
        _db = db;
    }

    private async Task<SetlistEntity> ToSetlistEntity(server.Data.Entities.SetlistEntity r)
    {
        var songIds = await _db.SetlistSongs
            .Where(ss => ss.SetlistId == r.SetlistId)
            .OrderBy(ss => ss.PositionIndex)
            .Select(ss => ss.SongId)
            .ToListAsync();

        return new SetlistEntity
        {
            Id = r.SetlistId,
            BandId = r.BandId,
            Name = r.Name,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            SongIds = songIds
        };
    }

    public async Task<IReadOnlyCollection<SetlistEntity>> GetByBandIdAsync(string bandId)
    {
        var rows = await _db.Setlists.Where(s => s.BandId == bandId).ToListAsync();
        var result = new List<SetlistEntity>();
        foreach (var r in rows)
        {
            result.Add(await ToSetlistEntity(r));
        }
        return result;
    }

    public async Task<SetlistEntity?> GetByIdAsync(string setlistId)
    {
        var r = await _db.Setlists.FirstOrDefaultAsync(s => s.SetlistId == setlistId);
        if (r == null) return null;
        return await ToSetlistEntity(r);
    }

    public async Task<SetlistEntity> CreateAsync(string bandId, string name)
    {
        var r = new server.Data.Entities.SetlistEntity
        {
            SetlistId = Guid.NewGuid().ToString(),
            BandId = bandId,
            Name = name,
            CreatedAt = DateTime.UtcNow
        };
        _db.Setlists.Add(r);
        await _db.SaveChangesAsync();
        return await ToSetlistEntity(r);
    }

    public async Task AddSongAsync(string setlistId, string songId)
    {
        var maxPosition = await _db.SetlistSongs
            .Where(ss => ss.SetlistId == setlistId)
            .Select(ss => (int?)ss.PositionIndex)
            .MaxAsync() ?? -1;

        var entry = new server.Data.Entities.SetlistSongEntity
        {
            SetlistSongId = Guid.NewGuid().ToString(),
            SetlistId = setlistId,
            SongId = songId,
            PositionIndex = maxPosition + 1,
            CreatedAt = DateTime.UtcNow
        };
        _db.SetlistSongs.Add(entry);
        await _db.SaveChangesAsync();
    }
}
