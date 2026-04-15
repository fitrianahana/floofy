namespace floofy.Models;

public class Review : Entity
{
  public int Rating { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public int HelpfulCount { get; set; } = 0;

  public Guid? ProductId { get; set; }
  public Guid? PetId { get; set; }
  public Guid BuyerId { get; set; }

  public bool IsValidRating()
  {
    return Rating >= 1 && Rating <= 5;
  }

  public void MarkAsHelpful()
  {
    HelpfulCount++;
    MarkAsUpdated();
  }
}