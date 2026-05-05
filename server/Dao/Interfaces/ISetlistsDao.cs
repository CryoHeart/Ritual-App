using server.Dao.Entities;

namespace server.Dao.Interfaces;

public interface ISetlistsDao
{
    Task<List<SetlistEntity>> GetSetlistsByBandIdAsync(string bandId);

    Task<SetlistEntity?> GetSetlistByIdAsync(string bandId, string setlistId);

    Task<bool> BandExistsAsync(string bandId);

    Task<SetlistEntity> CreateSetlistAsync(SetlistEntity setlist);

    Task<SetlistEntity?> UpdateSetlistAsync(SetlistEntity setlist);

    Task<bool> DeleteSetlistAsync(string bandId, string setlistId);

    Task<int> GetSetlistSongCountAsync(string setlistId);

    Task<int> GetSetlistTotalDurationSecondsAsync(string setlistId);

    Task<SetlistEntity?> GetByIdWithSongsAsync(string setlistId);

    Task<SetlistEntity> AddSongAsync(string setlistId, string songId);

    Task<SetlistEntity> RemoveSongAsync(string setlistId, string setlistSongId);

    Task<SetlistEntity> ReorderSongsAsync(string setlistId, IReadOnlyCollection<(string SetlistSongId, int PositionIndex)> order);
}
