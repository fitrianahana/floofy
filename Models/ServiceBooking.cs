namespace floofy.Models;

public class ServiceBooking : Entity
{
  public DateTime BookingDate { get; set; }
  public DateTime StartTime { get; set; }
  public DateTime EndTime { get; set; }
  public string Status { get; set; } = "Pending";
  public string? Notes { get; set; }

  public Guid BuyerId { get; set; }
  public Guid ServicePackageId { get; set; }
  public Guid? PaymentId { get; set; }

  public bool CanBeCancelled()
  {
    return Status == "Pending" || Status == "Confirmed";
  }

  public void CancelBooking()
  {
    if (CanBeCancelled())
    {
      Status = "Cancelled";
      MarkAsUpdated();
    }
  }

  public void ConfirmBooking()
  {
    Status = "Confirmed";
    MarkAsUpdated();
  }

  public void CompleteBooking()
  {
    Status = "Completed";
    MarkAsUpdated();
  }
}