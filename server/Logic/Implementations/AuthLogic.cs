using server.Dao.Interfaces;
using server.Data.Entities;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class AuthLogic : IAuthLogic
{
    private readonly IUsersDao _usersDao;
    private readonly IBandsDao _bandsDao;

    public AuthLogic(IUsersDao usersDao, IBandsDao bandsDao)
    {
        _usersDao = usersDao;
        _bandsDao = bandsDao;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return null;

        var user = await _usersDao.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (user is null || string.IsNullOrEmpty(user.PasswordHash))
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return new LoginResponse
        {
            UserId = user.UserId,
            DisplayName = user.DisplayName,
            Email = user.Email,
        };
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var displayName = request.DisplayName?.Trim() ?? string.Empty;
        var email = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        var password = request.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ValidationException("Display name is required.");
        if (displayName.Length > 100)
            throw new ValidationException("Display name must be 100 characters or fewer.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email is required.");
        if (email.Length > 150)
            throw new ValidationException("Email must be 150 characters or fewer.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ValidationException("Password is required.");
        if (password.Length < 8)
            throw new ValidationException("Password must be at least 8 characters.");

        var existingUser = await _usersDao.GetByEmailAsync(email);
        if (existingUser is not null)
            throw new ConflictException("An account with this email already exists.");

        var createdUser = new UserEntity
        {
            UserId = Guid.NewGuid().ToString(),
            DisplayName = displayName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow,
        };

        await _usersDao.CreateAsync(createdUser);
        await _bandsDao.CreateAsync(displayName, createdUser.UserId);

        return new LoginResponse
        {
            UserId = createdUser.UserId,
            DisplayName = createdUser.DisplayName,
            Email = createdUser.Email,
        };
    }
}
