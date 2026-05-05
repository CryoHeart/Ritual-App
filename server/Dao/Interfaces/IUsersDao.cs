using server.Data.Entities;

namespace server.Dao.Interfaces;

public interface IUsersDao
{
    Task<UserEntity?> GetByEmailAsync(string email);
    Task CreateAsync(UserEntity user);
}
