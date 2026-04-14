namespace floofy.Models;

public class PaymentMethod : Entity
{
  public string Type { get; set; } = string.Empty;
  public string LastFourDigits { get; set; } = string.Empty;
  public bool IsDefault { get; set; } = false;
  public DateTime? ExpiryDate { get; set; }

  public Guid UserId { get; set; }

  public bool IsExpired()
  {
    return ExpiryDate.HasValue && ExpiryDate < DateTime.UtcNow;
  }

  public string Mask()
  {
    return $"{Type} ending in {LastFourDigits}";
  }
}