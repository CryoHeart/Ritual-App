namespace server.Dao.Interfaces;

public interface IPermissionsDao
{
    Task<bool> UserBelongsToBandAsync(string userId, string bandId);
    Task<string?> GetUserBandRoleAsync(string userId, string bandId);
}
