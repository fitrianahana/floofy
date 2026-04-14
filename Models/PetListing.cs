namespace floofy.Models;

public class PetListing : Entity
{
  public decimal Price { get; set; }
  public string Currency { get; set; } = string.Empty;
  public bool IsActive { get; set; } = true;
  public DateTime ListingStartDate { get; set; }
  public DateTime? ListingEndDate { get; set; }
  public int Views { get; set; } = 0;

  public Guid PetId { get; set; }
  public Guid SellerId { get; set; }

  public bool IsCurrentlyActive()
  {
    return IsActive && (ListingEndDate == null || ListingEndDate > DateTime.UtcNow);
  }

  public void ExpireListing()
  {
    IsActive = false;
    ListingEndDate = DateTime.UtcNow;
    MarkAsUpdated();
  }

  public void IncreaseViewCount()
  {
    Views++;
    MarkAsUpdated();
  }
}