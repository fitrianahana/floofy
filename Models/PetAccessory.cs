namespace floofy.Models;

public class PetAccessory : Product
{
  public string Material { get; set; } = string.Empty;
  public string Color { get; set; } = string.Empty;
  public string Size { get; set; } = string.Empty;
  public string CompatibilityInfo { get; set; } = string.Empty;
}