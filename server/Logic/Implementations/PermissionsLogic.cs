using server.Dao.Interfaces;
using server.Logic.Exceptions;
using server.Logic.Interfaces;

namespace server.Logic.Implementations;

public class PermissionsLogic : IPermissionsLogic
{
    private const string RitualLeader = "RitualLeader";
    private const string BandMember   = "BandMember";

    private readonly IPermissionsDao _permissionsDao;

    public PermissionsLogic(IPermissionsDao permissionsDao)
    {
        _permissionsDao = permissionsDao;
    }

    public async Task<string?> GetUserBandRoleAsync(string userId, string bandId)
        => await _permissionsDao.GetUserBandRoleAsync(userId, bandId);

    public async Task EnsureBandMemberAsync(string userId, string bandId)
    {
        var role = await _permissionsDao.GetUserBandRoleAsync(userId, bandId);
        if (role is null)
            throw new ForbiddenException("You are not a member of this band.");
    }

    public async Task EnsureRitualLeaderAsync(string userId, string bandId)
    {
        var role = await _permissionsDao.GetUserBandRoleAsync(userId, bandId);
        if (role is null)
            throw new ForbiddenException("You are not a member of this band.");
        if (role != RitualLeader)
            throw new ForbiddenException("Only a Ritual Leader can perform this action.");
    }

    public async Task<bool> CanEditBandDataAsync(string userId, string bandId)
    {
        var role = await _permissionsDao.GetUserBandRoleAsync(userId, bandId);
        return role == RitualLeader;
    }

    public async Task<bool> CanViewBandDataAsync(string userId, string bandId)
    {
        var role = await _permissionsDao.GetUserBandRoleAsync(userId, bandId);
        return role == RitualLeader || role == BandMember;
    }

    public async Task<bool> CanControlLiveRitualAsync(string userId, string bandId)
    {
        var role = await _permissionsDao.GetUserBandRoleAsync(userId, bandId);
        return role == RitualLeader;
    }
}
