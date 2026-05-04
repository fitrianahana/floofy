namespace floofy.Services;

using floofy.Data;
using floofy.Models;

public class CartService : ICartService
{
  private readonly IRepository<Cart> _cartRepository;
  private readonly IRepository<CartItem> _cartItemRepository;
  private readonly IRepository<PetCartItem> _petCartItemRepository;
  private readonly IRepository<Product> _productRepository;

  public CartService(
      IRepository<Cart> cartRepository,
      IRepository<CartItem> cartItemRepository,
      IRepository<PetCartItem> petCartItemRepository,
      IRepository<Product> productRepository)
  {
    _cartRepository = cartRepository;
    _cartItemRepository = cartItemRepository;
    _petCartItemRepository = petCartItemRepository;
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

    var allCartItems = await _cartItemRepository.GetAllAsync();
    cart.Items = allCartItems
        .Where(ci => ci.CartId == cart.Id && !ci.IsDeleted)
        .ToList();

    var allPetCartItems = await _petCartItemRepository.GetAllAsync();
    cart.PetItems = allPetCartItems
        .Where(pi => pi.CartId == cart.Id && !pi.IsDeleted)
        .ToList();

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

    var allPetCartItems = await _petCartItemRepository.GetAllAsync();
    var petCartItems = allPetCartItems.Where(pi => pi.CartId == cart.Id && !pi.IsDeleted).ToList();
    foreach (var item in petCartItems)
    {
      item.MarkAsDeleted();
      await _petCartItemRepository.UpdateAsync(item);
    }
  }

  public async Task<bool> AddPetToCartAsync(Guid userId, Guid petId, decimal adoptionFee)
  {
    var cart = await GetUserCartAsync(userId);

    var allPetCartItems = await _petCartItemRepository.GetAllAsync();
    var existing = allPetCartItems.FirstOrDefault(pi =>
        pi.CartId == cart.Id && pi.PetId == petId && !pi.IsDeleted);

    if (existing != null)
    {
      return false;
    }

    var petCartItem = new PetCartItem
    {
      CartId = cart.Id,
      PetId = petId,
      AdoptionFee = adoptionFee
    };
    await _petCartItemRepository.InsertAsync(petCartItem);
    return true;
  }

  public async Task RemovePetFromCartAsync(Guid userId, Guid petCartItemId)
  {
    var item = (await _petCartItemRepository.GetByIdAsync(petCartItemId))!;
    item.MarkAsDeleted();
    await _petCartItemRepository.UpdateAsync(item);
  }

  public async Task<bool> IsPetInCartAsync(Guid userId, Guid petId)
  {
    var cart = await GetUserCartAsync(userId);
    return cart.PetItems.Any(pi => pi.PetId == petId);
  }
}