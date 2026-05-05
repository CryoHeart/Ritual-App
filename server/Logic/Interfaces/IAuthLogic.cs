using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface IAuthLogic
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
}
