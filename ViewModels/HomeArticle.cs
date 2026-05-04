namespace floofy.ViewModels;

public class HomeArticle
{
  public Guid Id { get; init; } = Guid.NewGuid();
  public string Title { get; init; } = string.Empty;
  public string Category { get; init; } = string.Empty;
  public string Thumbnail { get; init; } = string.Empty;
  public string ReadTime { get; init; } = string.Empty;
  public string Author { get; init; } = string.Empty;
  public string Excerpt { get; init; } = string.Empty;
}
