using Microsoft.EntityFrameworkCore;
using server.Dao.Interfaces;
using server.Data;
using server.Data.Entities;

namespace server.Dao.Implementations;

public class UsersDao : IUsersDao
{
    private readonly RitualDbContext _db;

    public UsersDao(RitualDbContext db)
    {
        _db = db;
    }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task CreateAsync(UserEntity user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
}
