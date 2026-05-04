namespace floofy.ViewModels;

public class HomeReview
{
  public Guid Id { get; init; } = Guid.NewGuid();
  public string AuthorName { get; init; } = string.Empty;
  public string Avatar { get; init; } = string.Empty;
  public int Rating { get; init; }
  public string Comment { get; init; } = string.Empty;
  public string Subject { get; init; } = string.Empty;
  public string TimeAgo { get; init; } = string.Empty;
}
