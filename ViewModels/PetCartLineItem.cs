namespace floofy.ViewModels;

public class PetCartLineItem
{
  public Guid PetCartItemId { get; init; }
  public Guid PetId { get; init; }
  public string PetName { get; init; } = string.Empty;
  public string Thumbnail { get; init; } = string.Empty;
  public string Species { get; init; } = string.Empty;
  public string Breed { get; init; } = string.Empty;
  public decimal AdoptionFee { get; init; }
}
