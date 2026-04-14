using floofy.Models.Enums;
namespace floofy.Models;

public class User : Entity
{
  public string FullName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public string ProfileImageUrl { get; set; } = string.Empty;
  public List<RoleType> Roles { get; set; } = new();
  public bool IsVerified { get; set; } = false;
  public string? VerificationToken { get; set; }

  public Guid? AddressId { get; set; }
  public Guid? BankAccountId { get; set; }

  public bool IsBuyer => Roles.Contains(RoleType.Buyer);
  public bool IsSeller => Roles.Contains(RoleType.Seller);

  public void AddRole(RoleType role)
  {
    if (!Roles.Contains(role))
    {
      Roles.Add(role);
      MarkAsUpdated();
    }
  }

  public void RemoveRole(RoleType role)
  {
    if (Roles.Contains(role))
    {
      Roles.Remove(role);
      MarkAsUpdated();
    }
  }

  public bool IsInRole(RoleType role)
  {
    return Roles.Contains(role);
  }
}