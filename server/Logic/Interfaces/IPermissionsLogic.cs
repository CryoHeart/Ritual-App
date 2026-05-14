namespace server.Logic.Interfaces;

public interface IPermissionsLogic
{
    Task EnsureBandMemberAsync(string userId, string bandId);
    Task EnsureRitualLeaderAsync(string userId, string bandId);
    Task<bool> CanEditBandDataAsync(string userId, string bandId);
    Task<bool> CanViewBandDataAsync(string userId, string bandId);
    Task<bool> CanControlLiveRitualAsync(string userId, string bandId);
    Task<string?> GetUserBandRoleAsync(string userId, string bandId);
}
