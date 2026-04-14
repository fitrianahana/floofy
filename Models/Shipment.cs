namespace floofy.Models;

public class Shipment : Entity
{
  public string TrackingNumber { get; set; } = string.Empty;
  public DateTime ShippedDate { get; set; }
  public DateTime? EstimatedDeliveryDate { get; set; }
  public DateTime? ActualDeliveryDate { get; set; }
  public string? Notes { get; set; }

  public Guid ShippingMethodId { get; set; }
  public Guid OrderId { get; set; }

  public bool IsDelivered()
  {
    return ActualDeliveryDate.HasValue;
  }

  public bool IsOverdue()
  {
    return !IsDelivered() && EstimatedDeliveryDate < DateTime.UtcNow;
  }

  public int GetDaysInTransit()
  {
    DateTime endDate = ActualDeliveryDate ?? DateTime.UtcNow;
    return (int)(endDate - ShippedDate).TotalDays;
  }
}