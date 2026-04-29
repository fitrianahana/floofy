using System.Security.Cryptography;
using System.Text;
using floofy.Data;
using floofy.Models;

namespace floofy.Services;

public class AuthService : IAuthService
{
  private readonly IRepository<User> _userRepository;

  public AuthService(IRepository<User> userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<User?> LoginAsync(LoginRequest request)
  {
    var email = request.Email.Trim();
    var inputPasswordHash = HashPassword(request.Password);

    var users = await _userRepository.GetAllAsync();
    var user = users.FirstOrDefault(u =>
      !u.IsDeleted &&
      u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    if (user is null)
      return null;

    user.SyncRolesFromJson();

    // Accept both hashed and legacy plaintext passwords.
    var isHashedMatch = user.Password == inputPasswordHash;
    var isLegacyPlainMatch = user.Password == request.Password;

    if (!isHashedMatch && !isLegacyPlainMatch)
      return null;

    return user;
  }

  public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
  {
    var email = request.Email.Trim();

    if (request.Roles.Count == 0)
      return (false, "Choose at least one role.");

    var users = await _userRepository.GetAllAsync();
    var exists = users.Any(u =>
      !u.IsDeleted &&
      u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    if (exists)
      return (false, "Email is already registered");

    var newUser = new User
    {
      FullName = request.FullName.Trim(),
      Email = email,
      Password = HashPassword(request.Password),
      Roles = request.Roles,
      IsVerified = true,
      PhoneNumber = string.Empty,
      ProfileImageUrl = string.Empty
    };

    await _userRepository.InsertAsync(newUser);
    return (true, "Registration successful.");
  }

  private static string HashPassword(string password)
  {
    using var sha = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(password);
    var hash = sha.ComputeHash(bytes);
    return Convert.ToHexString(hash);
  }
}
