using server.Dao.Entities;

namespace server.Dao.Interfaces;

public interface ILiveSessionsDao
{
    Task<LiveSessionEntity> CreateAsync(string bandId, string setlistId, string startedByUserId);

    Task<LiveSessionEntity?> GetByIdAsync(string sessionId);

    Task<LiveSessionEntity?> GetActiveByBandIdAsync(string bandId);

    Task UpdateAsync(LiveSessionEntity session);
}