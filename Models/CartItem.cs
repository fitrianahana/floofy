namespace floofy.Models;

public class CartItem : Entity
{
  public Guid CartId { get; set; }
  public Guid ProductId { get; set; }
  public int Quantity { get; set; }
  public decimal PriceAtAddTime { get; set; }
}