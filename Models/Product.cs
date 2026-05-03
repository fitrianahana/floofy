using SQLite;

namespace floofy.Models;

public class Product : Entity
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public decimal Price { get; set; }
  public int StockQuantity { get; set; }
  public string Sku { get; set; } = string.Empty;
  public string Thumbnail {get; set;} = string.Empty;
  
  [Ignore]
  public List<string> ImageUrls { get; set; } = new();
  public bool IsActive { get; set; } = true;
  public decimal Discount { get; set; } = 0;
  public int Rating { get; set; } = 0;

  public Guid SellerId { get; set; }
  public Guid ProductCategoryId { get; set; }

  [Ignore]
  public decimal DiscountedPrice => GetDiscountedPrice();

  [Ignore]
  public bool HasDiscount => Discount > 0;

  [Ignore]
  public bool InStock => StockQuantity > 0;

  public decimal GetDiscountedPrice()
  {
    return Price - (Price * (Discount / 100));
  }

  public bool IsInStock()
  {
    return StockQuantity > 0;
  }

  public bool ReduceStock(int quantity)
  {
    if (StockQuantity >= quantity)
    {
      StockQuantity -= quantity;
      MarkAsUpdated();
      return true;
    }
    return false;
  }

  public void ReplenishStock(int quantity)
  {
    StockQuantity += quantity;
    MarkAsUpdated();
  }

  public ProductListing GenerateListing()
  {
    return new ProductListing
    {
      ProductId = this.Id,
      SellerId = this.SellerId,
      ListingPrice = this.GetDiscountedPrice(),
      QuantityListed = this.StockQuantity,
      IsActive = true,
      ListingStartDate = DateTime.UtcNow
    };
  }
}