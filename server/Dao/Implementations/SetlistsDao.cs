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

    // Lightweight: returns setlist without songs (for list endpoints)
    private SetlistEntity ToSetlistEntity(server.Data.Entities.SetlistEntity r) => new SetlistEntity
    {
        Id = r.SetlistId,
        BandId = r.BandId,
        Name = r.Name,
        Description = r.Description,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };

    // Full: joins setlist_songs + songs + albums
    private async Task<SetlistEntity> ToSetlistEntityWithSongsAsync(server.Data.Entities.SetlistEntity r)
    {
        var entries = await (
            from ss in _db.SetlistSongs
            join s in _db.Songs on ss.SongId equals s.SongId
            join a in _db.Albums on s.AlbumId equals a.AlbumId into albumJoin
            from a in albumJoin.DefaultIfEmpty()
            where ss.SetlistId == r.SetlistId
            orderby ss.PositionIndex
            select new SetlistSongEntity
            {
                SetlistSongId = ss.SetlistSongId,
                SetlistId = ss.SetlistId,
                SongId = ss.SongId,
                PositionIndex = ss.PositionIndex,
                TransitionNotes = ss.TransitionNotes,
                PerformanceNotes = ss.PerformanceNotes,
                Title = s.Title,
                Bpm = s.Bpm,
                DurationSeconds = s.DurationSeconds,
                Tuning = s.Tuning,
                SongKey = s.SongKey,
                Notes = s.Notes,
                AlbumId = s.AlbumId,
                AlbumTitle = a != null ? a.Title : null,
                AlbumTrackNumber = s.AlbumTrackNumber
            }
        ).ToListAsync();

        var entity = ToSetlistEntity(r);
        entity.Songs = entries;
        entity.TotalSongs = entries.Count;
        entity.TotalDurationSeconds = entries.Sum(e => e.DurationSeconds ?? 0);
        return entity;
    }

    public async Task<List<SetlistEntity>> GetSetlistsByBandIdAsync(string bandId)
    {
        var rows = await _db.Setlists.Where(s => s.BandId == bandId).ToListAsync();

        // Get song counts and duration sums in one query
        var setlistIds = rows.Select(r => r.SetlistId).ToList();
        var songStats = await (
            from ss in _db.SetlistSongs
            join s in _db.Songs on ss.SongId equals s.SongId
            where setlistIds.Contains(ss.SetlistId)
            group new { ss, s } by ss.SetlistId into g
            select new
            {
                SetlistId = g.Key,
                Count = g.Count(),
                TotalDuration = g.Sum(x => (int?)(x.s.DurationSeconds ?? 0)) ?? 0
            }
        ).ToDictionaryAsync(x => x.SetlistId);

        return rows.Select(r =>
        {
            var stats = songStats.TryGetValue(r.SetlistId, out var st) ? st : null;
            var entity = ToSetlistEntity(r);
            entity.TotalSongs = stats?.Count ?? 0;
            entity.TotalDurationSeconds = stats?.TotalDuration ?? 0;
            return entity;
        }).ToList();
    }

    public async Task<SetlistEntity?> GetSetlistByIdAsync(string bandId, string setlistId)
    {
        var r = await _db.Setlists.FirstOrDefaultAsync(s => s.SetlistId == setlistId && s.BandId == bandId);
        if (r == null)
        {
            return null;
        }

        var entity = ToSetlistEntity(r);
        entity.TotalSongs = await GetSetlistSongCountAsync(setlistId);
        entity.TotalDurationSeconds = await GetSetlistTotalDurationSecondsAsync(setlistId);
        return entity;
    }

    public Task<bool> BandExistsAsync(string bandId)
    {
        return _db.Bands.AnyAsync(b => b.BandId == bandId);
    }

    public async Task<SetlistEntity?> GetByIdWithSongsAsync(string setlistId)
    {
        var r = await _db.Setlists.FirstOrDefaultAsync(s => s.SetlistId == setlistId);
        return r == null ? null : await ToSetlistEntityWithSongsAsync(r);
    }

    public async Task<SetlistEntity> CreateSetlistAsync(SetlistEntity setlist)
    {
        var r = new server.Data.Entities.SetlistEntity
        {
            SetlistId = Guid.NewGuid().ToString(),
            BandId = setlist.BandId,
            Name = setlist.Name,
            Description = setlist.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
        _db.Setlists.Add(r);
        await _db.SaveChangesAsync();

        return new SetlistEntity
        {
            Id = r.SetlistId,
            BandId = r.BandId,
            Name = r.Name,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            TotalSongs = 0,
            TotalDurationSeconds = 0
        };
    }

    public async Task<SetlistEntity?> UpdateSetlistAsync(SetlistEntity setlist)
    {
        var existing = await _db.Setlists
            .FirstOrDefaultAsync(s => s.SetlistId == setlist.Id && s.BandId == setlist.BandId);
        if (existing == null)
        {
            return null;
        }

        existing.Name = setlist.Name;
        existing.Description = setlist.Description;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new SetlistEntity
        {
            Id = existing.SetlistId,
            BandId = existing.BandId,
            Name = existing.Name,
            Description = existing.Description,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = existing.UpdatedAt,
            TotalSongs = await GetSetlistSongCountAsync(existing.SetlistId),
            TotalDurationSeconds = await GetSetlistTotalDurationSecondsAsync(existing.SetlistId)
        };
    }

    public async Task<bool> DeleteSetlistAsync(string bandId, string setlistId)
    {
        var setlist = await _db.Setlists
            .FirstOrDefaultAsync(s => s.SetlistId == setlistId && s.BandId == bandId);
        if (setlist == null)
        {
            return false;
        }

        _db.Setlists.Remove(setlist);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<int> GetSetlistSongCountAsync(string setlistId)
    {
        return _db.SetlistSongs.CountAsync(ss => ss.SetlistId == setlistId);
    }

    public async Task<int> GetSetlistTotalDurationSecondsAsync(string setlistId)
    {
        return await (
            from ss in _db.SetlistSongs
            join s in _db.Songs on ss.SongId equals s.SongId
            where ss.SetlistId == setlistId
            select (int?)(s.DurationSeconds ?? 0)
        ).SumAsync() ?? 0;
    }

    public async Task<SetlistEntity> AddSongAsync(string setlistId, string songId)
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

        var setlist = await _db.Setlists.FirstAsync(s => s.SetlistId == setlistId);
        return await ToSetlistEntityWithSongsAsync(setlist);
    }

    public async Task<SetlistEntity> RemoveSongAsync(string setlistId, string setlistSongId)
    {
        var entry = await _db.SetlistSongs
            .FirstOrDefaultAsync(ss => ss.SetlistSongId == setlistSongId && ss.SetlistId == setlistId);

        if (entry != null)
        {
            _db.SetlistSongs.Remove(entry);
            await _db.SaveChangesAsync();

            // Reindex remaining songs from 0..n using raw SQL to avoid EF Core
            // batching collisions on the UNIQUE KEY (setlist_id, position_index).
            var remaining = await _db.SetlistSongs
                .Where(ss => ss.SetlistId == setlistId)
                .OrderBy(ss => ss.PositionIndex)
                .Select(ss => ss.SetlistSongId)
                .ToListAsync();

            if (remaining.Count > 0)
            {
                int offset = remaining.Count + 1000;
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE setlist_songs SET position_index = position_index + {offset} WHERE setlist_id = {setlistId}");

                for (int i = 0; i < remaining.Count; i++)
                {
                    var id = remaining[i];
                    await _db.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE setlist_songs SET position_index = {i} WHERE setlist_song_id = {id} AND setlist_id = {setlistId}");
                }

                _db.ChangeTracker.Clear();
            }
        }

        var setlist = await _db.Setlists.FirstAsync(s => s.SetlistId == setlistId);
        return await ToSetlistEntityWithSongsAsync(setlist);
    }

    public async Task<SetlistEntity> ReorderSongsAsync(string setlistId, IReadOnlyCollection<(string SetlistSongId, int PositionIndex)> order)
    {
        // Use raw SQL to avoid EF Core batching multiple UPDATEs into one round-trip,
        // which causes MySQL to hit the unique constraint (setlist_id, position_index)
        // mid-batch even when the final state would be valid.
        int offset = order.Count + 1000;

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            // Step 1: single SQL statement shifts ALL rows to a safe temp range.
            // MySQL evaluates each row in sequence; adding offset never creates a duplicate
            // because new values (n+1000+) don't overlap with existing values (0..n-1).
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE setlist_songs SET position_index = position_index + {offset} WHERE setlist_id = {setlistId}");

            // Step 2: one statement per row sets the final position.
            // All other rows are still in the shifted range, so no collision is possible.
            foreach (var (id, pos) in order)
            {
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE setlist_songs SET position_index = {pos} WHERE setlist_song_id = {id} AND setlist_id = {setlistId}");
            }

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        // Clear the EF tracker since we bypassed it with raw SQL.
        _db.ChangeTracker.Clear();
        var setlist = await _db.Setlists.FirstAsync(s => s.SetlistId == setlistId);
        return await ToSetlistEntityWithSongsAsync(setlist);
    }
}
