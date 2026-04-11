using floofy.Models;

namespace floofy.Services;

public class MockAuthService : IAuthService
{
  private readonly List<User> _users = new()
  {
    new User
    {
      FullName = "Frans Jesky",
      Email = "jesky@floofy.com",
      Password = "jesky",
      Roles = new List<RoleType> {RoleType.Buyer, RoleType.Seller}
    },
    new User
    {
      FullName = "Julian Asna",
      Email = "julian@floofy.com",
      Password = "julian",
      Roles = new List<RoleType> {RoleType.Buyer, RoleType.Seller}
    },
    new User
    {
      FullName = "Buyer Seller",
      Email = "both@floofy.com",
      Password = "123456",
      Roles = new List<RoleType> {RoleType.Buyer, RoleType.Seller}
    }
  };

  public Task<User?> LoginAsync(LoginRequest request)
  {
    var user = _users.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) && u.Password == request.Password);

    return Task.FromResult(user);
  }

  public Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
  {
    if (_users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
      return Task.FromResult((false, "Email is already registered"));

    if (request.Roles.Count == 0)
      return Task.FromResult((false, "Choose at least one role."));

    _users.Add(new User
    {
      FullName = request.FullName,
      Email = request.Email,
      Password = request.Password,
      Roles = request.Roles
    });

    return Task.FromResult((true, "Registration successful."));
  }
}