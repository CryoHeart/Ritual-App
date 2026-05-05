using server.Dao.Entities;

namespace server.Dao.Interfaces;

public interface IAlbumsDao
{
    Task<bool> BandExistsAsync(string bandId);

    Task<bool> AlbumExistsForBandAsync(string bandId, string albumId);

    Task<IReadOnlyCollection<AlbumEntity>> GetByBandIdAsync(string bandId);

    Task<IReadOnlyCollection<AlbumEntity>> GetByBandIdWithSongsAsync(string bandId);

    Task<AlbumEntity?> GetByIdAsync(string albumId);

    Task<AlbumEntity> CreateAsync(string bandId, string title, int? releaseYear, int sortOrder);

    Task<AlbumEntity> UpdateAsync(string albumId, string title, int? releaseYear, int sortOrder);

    Task DeleteAsync(string albumId);
}
