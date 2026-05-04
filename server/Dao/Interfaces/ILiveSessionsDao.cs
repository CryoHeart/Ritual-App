using server.Dao.Entities;

namespace server.Dao.Interfaces;

public interface ILiveSessionsDao
{
    Task<LiveSessionEntity> CreateAsync(string bandId, string setlistId);

    Task<LiveSessionEntity?> GetByIdAsync(string sessionId);

    Task UpdateAsync(LiveSessionEntity session);
}