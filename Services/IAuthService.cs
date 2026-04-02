using floofy.Models;

namespace floofy.Services;

public interface IAuthService
{
  Task<User?> LoginAsync(LoginRequest request);
  Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request);
}