using floofy.Models.Enums;
namespace floofy.Models;

public class Post : Entity
{
  public string Title { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public List<string> ImageUrls { get; set; } = new();
  public int LikeCount { get; set; } = 0;
  public int CommentCount { get; set; } = 0;
  public PostVisibility Visibility { get; set; } = PostVisibility.Public;

  public Guid AuthorId { get; set; }

  public void IncrementLikeCount()
  {
    LikeCount++;
    MarkAsUpdated();
  }

  public void DecrementLikeCount()
  {
    if (LikeCount > 0)
    {
      LikeCount--;
      MarkAsUpdated();
    }
  }

  public void IncrementCommentCount()
  {
    CommentCount++;
    MarkAsUpdated();
  }

  public bool IsVisible()
  {
    return !IsDeleted && Visibility == PostVisibility.Public;
  }
}