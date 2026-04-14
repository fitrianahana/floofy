using floofy.Models.Enums;
namespace floofy.Models;

public class User
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string FullName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public List<RoleType> Roles { get; set; } = new();

  public bool IsBuyer => Roles.Contains(RoleType.Buyer);
  public bool IsSeller => Roles.Contains(RoleType.Seller);
}