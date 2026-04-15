using floofy.Models.Enums;
namespace floofy.Models;

public class EventRSVP : Entity
{
  public RSVPStatus RSVPStatus { get; set; } = RSVPStatus.Pending;
  public DateTime RegistrationDate { get; set; }

  public Guid AttendeeId { get; set; }
  public Guid EventId { get; set; }

  public bool CanBeCancelled()
  {
    return RSVPStatus == RSVPStatus.Pending || RSVPStatus == RSVPStatus.Attending;
  }

  public void CancelRSVP()
  {
    if (RSVPStatus == RSVPStatus.Pending || RSVPStatus == RSVPStatus.Attending)
    {
      RSVPStatus = RSVPStatus.Cancelled;
      MarkAsUpdated();
    }
  }

  public void ConfirmAttendance()
  {
    RSVPStatus = RSVPStatus.Attending;
    MarkAsUpdated();
  }
}