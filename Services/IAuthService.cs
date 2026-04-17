namespace floofy.Services;

using floofy.Models;

public interface IAuthService
{
  Task<User?> LoginAsync(LoginRequest request);
  Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request);
}