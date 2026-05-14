using server.Dao.Entities;

namespace server.Dao.Interfaces;

public interface IBandsDao
{
    Task<IReadOnlyCollection<BandEntity>> GetAllAsync();

    Task<IReadOnlyCollection<BandEntity>> GetByUserIdAsync(string userId);

    Task<BandEntity?> GetByIdAsync(string bandId);

    Task<BandEntity> CreateAsync(string name, string createdByUserId);

    Task<BandEntity?> UpdateNameAsync(string bandId, string name);

    Task<string?> GetUserRoleInBandAsync(string userId, string bandId);

    Task<IReadOnlyCollection<(string BandMemberId, string UserId, string DisplayName, string Email, string Role, string? Instrument, DateTime JoinedAt)>> GetMembersAsync(string bandId);

    Task AddMemberAsync(string bandId, string userId, string role, string? instrument);

    Task<bool> IsMemberAsync(string userId, string bandId);
}