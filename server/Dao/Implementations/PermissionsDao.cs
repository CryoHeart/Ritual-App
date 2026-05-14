using Microsoft.EntityFrameworkCore;
using server.Dao.Interfaces;
using server.Data;

namespace server.Dao.Implementations;

public class PermissionsDao : IPermissionsDao
{
    private readonly RitualDbContext _db;

    public PermissionsDao(RitualDbContext db)
    {
        _db = db;
    }

    public async Task<bool> UserBelongsToBandAsync(string userId, string bandId)
        => await _db.BandMembers.AnyAsync(m => m.UserId == userId && m.BandId == bandId);

    public async Task<string?> GetUserBandRoleAsync(string userId, string bandId)
        => await _db.BandMembers
            .Where(m => m.UserId == userId && m.BandId == bandId)
            .Select(m => m.Role)
            .FirstOrDefaultAsync();
}
