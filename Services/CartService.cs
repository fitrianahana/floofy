namespace floofy.Services;

using floofy.Data;
using floofy.Models;

public class CartService : ICartService
{
  private readonly IRepository<Cart> _cartRepository;
  private readonly IRepository<CartItem> _cartItemRepository;
  private readonly IRepository<Product> _productRepository;

  public CartService(
      IRepository<Cart> cartRepository,
      IRepository<CartItem> cartItemRepository,
      IRepository<Product> productRepository)
  {
    _cartRepository = cartRepository;
    _cartItemRepository = cartItemRepository;
    _productRepository = productRepository;
  }

  public async Task<Cart> GetUserCartAsync(Guid userId)
  {
    var allCarts = await _cartRepository.GetAllAsync();
    var cart = allCarts.FirstOrDefault(c => c.UserId == userId && !c.IsDeleted);

    if (cart == null)
    {
      cart = new Cart { UserId = userId };
      await _cartRepository.InsertAsync(cart);
    }
    return cart;
  }

  public async Task AddToCartAsync(Guid userId, Guid productId, int quantity)
  {
    var cart = await GetUserCartAsync(userId);
    var product = (await _productRepository.GetByIdAsync(productId))!;

    // Check if item already in cart
    var allCartItems = await _cartItemRepository.GetAllAsync();
    var existingItem = allCartItems.FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductId == productId && !ci.IsDeleted);

    if (existingItem != null)
    {
      // Update quantity
      existingItem.Quantity += quantity;
      existingItem.MarkAsUpdated();

      await _cartItemRepository.UpdateAsync(existingItem);
    }
    else
    {
      // Create new cart item
      var cartItem = new CartItem
      {
        CartId = cart.Id,
        ProductId = productId,
        Quantity = quantity,
        PriceAtAddTime = product.Price
      };

      await _cartItemRepository.InsertAsync(cartItem);
    }
  }

  public async Task RemoveFromCartAsync(Guid userId, Guid cartItemId)
  {
    var cartItem = (await _cartItemRepository.GetByIdAsync(cartItemId))!;
    cartItem.MarkAsDeleted();

    await _cartItemRepository.UpdateAsync(cartItem);
  }

  public async Task UpdateCartItemQuantityAsync(Guid cartItemId, int newQuantity)
  {
    var cartItem = (await _cartItemRepository.GetByIdAsync(cartItemId))!;
    cartItem.Quantity = newQuantity;
    cartItem.MarkAsUpdated();

    await _cartItemRepository.UpdateAsync(cartItem);
  }

  public async Task<decimal> GetCartTotalAsync(Guid userId)
  {
    var cart = await GetUserCartAsync(userId);
    var allCartItems = await _cartItemRepository.GetAllAsync();
    var cartItems = allCartItems.Where(ci => ci.CartId == cart.Id && !ci.IsDeleted).ToList();
    decimal total = 0m;

    foreach (var item in cartItems)
    {
      total += item.PriceAtAddTime * item.Quantity;
    }
    return total;
  }

  public async Task ClearCartAsync(Guid userId)
  {
    var cart = await GetUserCartAsync(userId);
    var allCartItems = await _cartItemRepository.GetAllAsync();
    var cartItems = allCartItems.Where(ci => ci.CartId == cart.Id && !ci.IsDeleted).ToList();

    foreach (var item in cartItems)
    {
      item.MarkAsDeleted();
      await _cartItemRepository.UpdateAsync(item);
    }
  }
}