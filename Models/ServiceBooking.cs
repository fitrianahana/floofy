using floofy.Models.Enums;
namespace floofy.Models;

public class ServiceBooking : Entity
{
  public DateTime BookingDate { get; set; }
  public DateTime StartTime { get; set; }
  public DateTime EndTime { get; set; }
  public BookingStatus Status { get; set; } = BookingStatus.Pending;
  public string? Notes { get; set; }

  public Guid BuyerId { get; set; }
  public Guid ServicePackageId { get; set; }
  public Guid? PaymentId { get; set; }

  public bool CanBeCancelled()
  {
    return Status == BookingStatus.Pending || Status == BookingStatus.Confirmed;
  }

  public void CancelBooking()
  {
    if (CanBeCancelled())
    {
      Status = BookingStatus.Cancelled;
      MarkAsUpdated();
    }
  }

  public void ConfirmBooking()
  {
    Status = BookingStatus.Confirmed;
    MarkAsUpdated();
  }

  public void CompleteBooking()
  {
    Status = BookingStatus.Completed;
    MarkAsUpdated();
  }
}