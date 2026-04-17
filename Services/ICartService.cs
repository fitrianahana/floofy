namespace floofy.Services;

using floofy.Models;

public interface ICartService
{
  Task<Cart> GetUserCartAsync(Guid userId);
  Task AddToCartAsync(Guid userId, Guid productId, int quantity);
  Task RemoveFromCartAsync(Guid userId, Guid cartItemId);
  Task UpdateCartItemQuantityAsync(Guid cartItemId, int newQuantity);
  Task<decimal> GetCartTotalAsync(Guid userId);
  Task ClearCartAsync(Guid userId);
}