namespace floofy.Models;

public class PetCartItem : Entity
{
  public Guid CartId { get; set; }
  public Guid PetId { get; set; }
  public decimal AdoptionFee { get; set; }
}
