namespace floofy.Models;

public class Cart : Entity
{
  public Guid UserId { get; set; }
  public List<CartItem> Items { get; set; } = new();
}