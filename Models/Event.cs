namespace floofy.Models;

public class Event : Entity
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public DateTime EventDate { get; set; }
  public string Location { get; set; } = string.Empty;
  public int MaxAttendees { get; set; } = 0;
  public int CurrentAttendees { get; set; } = 0;
  public string? ImageUrl { get; set; }

  public Guid OrganizerId { get; set; }

  public bool IsAvailable()
  {
    return CurrentAttendees < MaxAttendees && EventDate > DateTime.UtcNow;
  }

  public bool CanAddAttendee()
  {
    return CurrentAttendees < MaxAttendees;
  }

  public void AddAttendee()
  {
    if (CanAddAttendee())
    {
      CurrentAttendees++;
      MarkAsUpdated();
    }
  }

  public void RemoveAttendee()
  {
    if (CurrentAttendees > 0)
    {
      CurrentAttendees--;
      MarkAsUpdated();
    }
  }

  public bool IsEventPassed()
  {
    return EventDate < DateTime.UtcNow;
  }

  public int GetAvailableSlots()
  {
    return MaxAttendees - CurrentAttendees;
  }
}