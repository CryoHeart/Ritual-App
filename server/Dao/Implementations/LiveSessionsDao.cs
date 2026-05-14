using Microsoft.EntityFrameworkCore;
using server.Dao.Entities;
using server.Dao.Interfaces;
using server.Data;

namespace server.Dao.Implementations;

public class LiveSessionsDao : ILiveSessionsDao
{
    private readonly RitualDbContext _db;

    public LiveSessionsDao(RitualDbContext db)
    {
        _db = db;
    }

    private static LiveSessionEntity Map(server.Data.Entities.LiveSessionEntity r) => new LiveSessionEntity
    {
        Id = r.LiveSessionId,
        BandId = r.BandId,
        SetlistId = r.SetlistId,
        StartedByUserId = r.StartedByUserId,
        Status = r.Status,
        CurrentSongIndex = r.CurrentPositionIndex,
        StartedAt = r.StartedAt,
        EndedAt = r.EndedAt
    };

    public async Task<LiveSessionEntity> CreateAsync(string bandId, string setlistId, string startedByUserId)
    {
        var r = new server.Data.Entities.LiveSessionEntity
        {
            LiveSessionId = Guid.NewGuid().ToString(),
            BandId = bandId,
            SetlistId = setlistId,
            StartedByUserId = startedByUserId,
            Status = "active",
            CurrentPositionIndex = 0,
            StartedAt = DateTime.UtcNow
        };
        _db.LiveSessions.Add(r);
        await _db.SaveChangesAsync();
        var entity = Map(r);
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == startedByUserId);
        entity.StartedByDisplayName = user?.DisplayName;
        return entity;
    }

    public async Task<LiveSessionEntity?> GetByIdAsync(string sessionId)
    {
        var r = await _db.LiveSessions.FirstOrDefaultAsync(s => s.LiveSessionId == sessionId);
        if (r == null) return null;
        var entity = Map(r);
        if (r.StartedByUserId != null)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == r.StartedByUserId);
            entity.StartedByDisplayName = user?.DisplayName;
        }
        return entity;
    }

    public async Task<LiveSessionEntity?> GetActiveByBandIdAsync(string bandId)
    {
        var r = await _db.LiveSessions
            .Where(s => s.BandId == bandId && s.Status == "active")
            .OrderByDescending(s => s.StartedAt)
            .FirstOrDefaultAsync();
        if (r == null) return null;
        var entity = Map(r);
        if (r.StartedByUserId != null)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == r.StartedByUserId);
            entity.StartedByDisplayName = user?.DisplayName;
        }
        return entity;
    }

    public async Task UpdateAsync(LiveSessionEntity session)
    {
        var r = await _db.LiveSessions.FirstOrDefaultAsync(s => s.LiveSessionId == session.Id);
        if (r == null) return;

        r.CurrentPositionIndex = session.CurrentSongIndex;
        r.Status = session.Status;
        if (session.IsEnded) r.EndedAt = session.EndedAt ?? DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
