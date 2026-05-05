using server.Dao.Entities;

namespace server.Dao.Interfaces;

public interface ISongsDao
{
    Task<bool> BandExistsAsync(string bandId);

    Task<IReadOnlyCollection<SongEntity>> GetByBandIdAsync(string bandId);

    Task<SongEntity?> GetByIdAsync(string bandId, string songId);

    Task<SongEntity?> GetByIdAsync(string songId);

    Task<SongEntity> CreateAsync(
        string bandId,
        string title,
        string? albumId,
        int? albumTrackNumber,
        int? durationSeconds,
        int? bpm,
        string? tuning,
        string? songKey,
        string? notes);

    Task<SongEntity> UpdateAsync(
        string songId,
        string title,
        string? albumId,
        int? durationSeconds,
        int? albumTrackNumber,
        int? bpm,
        string? tuning,
        string? songKey,
        string? notes);

    Task DeleteAsync(string songId);
}