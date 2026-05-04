using server.Dao.Entities;

namespace server.Dao.Interfaces;

public interface IBandsDao
{
    Task<IReadOnlyCollection<BandEntity>> GetAllAsync();

    Task<BandEntity?> GetByIdAsync(string bandId);

    Task<BandEntity> CreateAsync(string name, string createdByUserId);
}