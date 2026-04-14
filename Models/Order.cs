using floofy.Models.Enums;

namespace floofy.Models;

public class Order : Entity
{
  public string OrderNumber { get; set; } = string.Empty;
  public DateTime OrderDate { get; set; }
  public decimal TotalPrice { get; set; }
  public decimal TotalTax { get; set; }
  public decimal TotalDiscount { get; set; }
  public decimal FinalPrice { get; set; }
  public OrderStatus Status { get; set; }
  public PaymentStatus PaymentStatus { get; set; }
  public ShippingStatus ShippingStatus { get; set; }
  public string? Notes { get; set; }

  public Guid BuyerId { get; set; }
  public Guid ShippingAddressId { get; set; }
  public Guid BillingAddressId { get; set; }
  public Guid? ShippingMethodId { get; set; }
  public Guid? PaymentMethodId { get; set; }

  public bool CanBeCancelled()
  {
    return Status == OrderStatus.Pending || Status == OrderStatus.Processing;
  }

  public bool CanBeReturned()
  {
    return Status == OrderStatus.Delivered && (DateTime.UtcNow - OrderDate).TotalDays <= 30;
  }

  public void CancelOrder()
  {
    if (CanBeCancelled())
    {
      Status = OrderStatus.Cancelled;
      PaymentStatus = PaymentStatus.Refunded;
      MarkAsUpdated();
    }
  }

  public void UpdateStatus(OrderStatus newStatus)
  {
    Status = newStatus;
    MarkAsUpdated();
  }
}