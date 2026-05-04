using server.Dao.Entities;

namespace server.Dao.Interfaces;

public interface ISetlistsDao
{
    Task<IReadOnlyCollection<SetlistEntity>> GetByBandIdAsync(string bandId);

    Task<SetlistEntity?> GetByIdAsync(string setlistId);

    Task<SetlistEntity> CreateAsync(string bandId, string name);

    Task AddSongAsync(string setlistId, string songId);
}