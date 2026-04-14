namespace floofy.Models;

public class ProductListing : Entity
{
  public decimal ListingPrice { get; set; }
  public int QuantityListed { get; set; }
  public bool IsActive { get; set; } = true;
  public DateTime ListingStartDate { get; set; }
  public DateTime? ListingEndDate { get; set; }
  public int Views { get; set; } = 0;

  public Guid ProductId { get; set; }
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