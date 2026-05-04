using server.Dao.Entities;

namespace server.Dao.Interfaces;

public interface ISongsDao
{
    Task<IReadOnlyCollection<SongEntity>> GetByBandIdAsync(string bandId);

    Task<SongEntity?> GetByIdAsync(string bandId, string songId);

    Task<SongEntity?> GetByIdAsync(string songId);

    Task<SongEntity> CreateAsync(string bandId, string title, int? bpm, int? durationSeconds);
}