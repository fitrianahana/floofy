namespace floofy.Models;

public class OrderLineItem : Entity
{
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public decimal TotalPrice { get; set; }
  public decimal Discount { get; set; }

  public Guid ProductId { get; set; }
  public Guid OrderId { get; set; }

  public void CalculateTotal()
  {
    TotalPrice = (UnitPrice * Quantity) - Discount;
  }
}