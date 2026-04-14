namespace floofy.Models;

public class Address : Entity
{
  public string Street { get; set; } = string.Empty;
  public string City { get; set; } = string.Empty;
  public string Province { get; set; } = string.Empty;
  public string PostalCode { get; set; } = string.Empty;
  public bool IsDefault { get; set; } = false;

  public Guid UserId { get; set; }

  public bool IsValid()
  {
    return !string.IsNullOrEmpty(Street)
    && !string.IsNullOrEmpty(City)
    && !string.IsNullOrEmpty(Province)
    && !string.IsNullOrEmpty(PostalCode);
  }

  public string GetFormattedAddress()
  {
    return $"{Street}, {City}, {Province} {PostalCode}";
  }
}