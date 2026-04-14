namespace floofy.Models;

public abstract class Entity
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  public bool IsDeleted { get; set; } = false;

  public virtual void MarkAsDeleted()
  {
    IsDeleted = true;
    UpdatedAt = DateTime.UtcNow;
  }

  public virtual void MarkAsUpdated()
  {
    UpdatedAt = DateTime.UtcNow;
  }
}