using SQLite;
using floofy.Models.Enums;

namespace floofy.Models;

public class Pet : Entity
{
  public string Name { get; set; } = string.Empty;
  public string Species { get; set; } = string.Empty;
  public string Breed { get; set; } = string.Empty;
  public int Age { get; set; }
  public Gender Gender { get; set; }
  public decimal Weight { get; set; }
  public decimal Height { get; set; }
  public string Description { get; set; } = string.Empty;
  [Ignore]
  public List<string> ImageUrls { get; set; } = new();
  public bool Vaccinated { get; set; } = false;
  public bool Neutered { get; set; } = false;
  public string? HealthCertificate { get; set; }

  public Guid SellerId { get; set; }
  public Guid PetCategoryId { get; set; }

  public bool IsAvailable()
  {
    return !IsDeleted;
  }

  public int GetAgeInMonths()
  {
    return Age * 12;
  }

  public PetListing GenerateListing()
  {
    return new PetListing
    {
      PetId = this.Id,
      SellerId = this.SellerId,
      IsActive = true,
      ListingStartDate = DateTime.UtcNow
    };
  }
}