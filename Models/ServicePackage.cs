namespace floofy.Models;

public class ServicePackage : Product
{
  public int Duration { get; set; }
  public int MaxCapacity { get; set; }
  public int CurrentBookings { get; set; } = 0;

  public Guid ServiceCategoryId { get; set; }

  public bool IsAvailable()
  {
    return IsActive && CurrentBookings < MaxCapacity;
  }

  public bool CanBookMoreSlots()
  {
    return CurrentBookings < MaxCapacity;
  }

  public void ReserveSlot()
  {
    if (CanBookMoreSlots())
    {
      CurrentBookings++;
      MarkAsUpdated();
    }
  }

  public void ReleaseSlot()
  {
    if (CurrentBookings > 0)
    {
      CurrentBookings--;
      MarkAsUpdated();
    }
  }
}