using SQLite;
using floofy.Models.Enums;
using System.Text.Json;

namespace floofy.Models;

public class User : Entity
{
  private List<RoleType> _roles = new();

  public string FullName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public string ProfileImageUrl { get; set; } = string.Empty;

  public string RolesJson { get; set; } = "[]";

  [Ignore]
  public List<RoleType> Roles
  {
    get => _roles;
    set
    {
      _roles = value ?? new List<RoleType>();
      RolesJson = JsonSerializer.Serialize(_roles);
    }
  }

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
      RolesJson = JsonSerializer.Serialize(Roles);
      MarkAsUpdated();
    }
  }

  public void RemoveRole(RoleType role)
  {
    if (Roles.Contains(role))
    {
      Roles.Remove(role);
      RolesJson = JsonSerializer.Serialize(Roles);
      MarkAsUpdated();
    }
  }

  public bool IsInRole(RoleType role)
  {
    return Roles.Contains(role);
  }

  public void SyncRolesFromJson()
  {
    try
    {
      _roles = string.IsNullOrWhiteSpace(RolesJson)
        ? new List<RoleType>()
        : JsonSerializer.Deserialize<List<RoleType>>(RolesJson) ?? new List<RoleType>();
    }
    catch
    {
      _roles = new List<RoleType>();
    }
  }
}
