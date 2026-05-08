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

    public async Task<LoginResponse> UpdateEmailAsync(UpdateEmailRequest request)
    {
        var userId = request.UserId?.Trim() ?? string.Empty;
        var newEmail = request.NewEmail?.Trim().ToLowerInvariant() ?? string.Empty;
        var currentPassword = request.CurrentPassword ?? string.Empty;

        if (string.IsNullOrWhiteSpace(userId))
            throw new ValidationException("User id is required.");
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ValidationException("Email is required.");
        if (newEmail.Length > 150)
            throw new ValidationException("Email must be 150 characters or fewer.");
        if (string.IsNullOrWhiteSpace(currentPassword))
            throw new ValidationException("Current password is required.");

        var user = await _usersDao.GetByIdAsync(userId);
        if (user is null)
            throw new NotFoundException("User was not found.");

        if (string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            throw new ValidationException("Current password is incorrect.");

        var existingUser = await _usersDao.GetByEmailAsync(newEmail);
        if (existingUser is not null && existingUser.UserId != user.UserId)
            throw new ConflictException("An account with this email already exists.");

        user.Email = newEmail;
        user.UpdatedAt = DateTime.UtcNow;
        await _usersDao.UpdateAsync(user);

        return new LoginResponse
        {
            UserId = user.UserId,
            DisplayName = user.DisplayName,
            Email = user.Email,
        };
    }

    public async Task UpdatePasswordAsync(UpdatePasswordRequest request)
    {
        var userId = request.UserId?.Trim() ?? string.Empty;
        var currentPassword = request.CurrentPassword ?? string.Empty;
        var newPassword = request.NewPassword ?? string.Empty;

        if (string.IsNullOrWhiteSpace(userId))
            throw new ValidationException("User id is required.");
        if (string.IsNullOrWhiteSpace(currentPassword))
            throw new ValidationException("Current password is required.");
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ValidationException("New password is required.");
        if (newPassword.Length < 8)
            throw new ValidationException("New password must be at least 8 characters.");

        var user = await _usersDao.GetByIdAsync(userId);
        if (user is null)
            throw new NotFoundException("User was not found.");

        if (string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            throw new ValidationException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _usersDao.UpdateAsync(user);
    }
}
