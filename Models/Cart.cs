using SQLite;

namespace floofy.Models;

public class Cart : Entity
{
  public Guid UserId { get; set; }
  [Ignore]
  public List<CartItem> Items { get; set; } = new();
}