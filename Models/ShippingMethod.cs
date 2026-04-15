namespace floofy.Models;

public class ShippingMethod : Entity
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public decimal BaseCost { get; set; }
  public int EstimatedDays { get; set; } = 1;
  public bool IsActive { get; set; } = true;

  public decimal CalculateShippingCost(decimal orderAmount)
  {
    return BaseCost;
  }
}