using floofy.Models.Enums;
namespace floofy.Models;

public class RegisterRequest
{
  public string FullName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public List<RoleType> Roles { get; set; } = new();
}