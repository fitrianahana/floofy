namespace floofy.Models;

public class BankAccount : Entity
{
  public string AccountHolderName { get; set; } = string.Empty;
  public string AccountNumber { get; set; } = string.Empty;
  public string RoutingNumber { get; set; } = string.Empty;
  public string BankName { get; set; } = string.Empty;
  public string AccountType { get; set; } = string.Empty;
  public bool IsVerified { get; set; } = false;
  public DateTime? VerificationDate { get; set; }

  public Guid UserId { get; set; }

  public bool IsValid()
  {
    return !string.IsNullOrEmpty(AccountHolderName) &&
    !string.IsNullOrEmpty(AccountNumber) &&
    !string.IsNullOrEmpty(RoutingNumber) &&
    !string.IsNullOrEmpty(BankName) &&
    !string.IsNullOrEmpty(AccountType);
  }

  public BankAccount Mask()
  {
    return new BankAccount
    {
      Id = this.Id,
      AccountHolderName = this.AccountHolderName,
      AccountNumber = $"****{this.AccountNumber.Substring(Math.Max(0, this.AccountNumber.Length - 4))}",
      RoutingNumber = "****",
      BankName = this.BankName,
      AccountType = this.AccountType,
      IsVerified = this.IsVerified,
      VerificationDate = this.VerificationDate,
      UserId = this.UserId
    };
  }
}