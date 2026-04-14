using floofy.Models.Enums;

namespace floofy.Models;

public class OrderHistory : Entity
{
  public OrderStatus PreviousStatus { get; set; }
  public OrderStatus NewStatus { get; set; }
  public DateTime Timestamp { get; set; }
  public string? Notes { get; set; }

  public Guid OrderId { get; set; }
}