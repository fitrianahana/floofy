using System.Collections.ObjectModel;
using floofy.Models;
using floofy.Services;
using System.Windows.Input;
namespace floofy.ViewModels;

public class CartViewModel : BaseViewModel
{
  private readonly ICartService _cartService;
  private readonly SessionService _sessionService;
  private ObservableCollection<CartItem> _cartItems = new();
  private decimal _totalPrice = 0m;
  private int _itemCount = 0;

  public ObservableCollection<CartItem> CartItems
  {
    get => _cartItems;
    set => SetProperty(ref _cartItems, value);
  }

  public decimal TotalPrice
  {
    get => _totalPrice;
    set => SetProperty(ref _totalPrice, value);
  }

  public int ItemCount
  {
    get => _itemCount;
    set => SetProperty(ref _itemCount, value);
  }

  public ICommand LoadCartCommand { get; }
  public ICommand AddToCartCommand { get; }
  public ICommand RemoveFromCartCommand { get; }
  public ICommand ClearCartCommand { get; }

  public CartViewModel()
  {
    _cartService = App.Services.GetRequiredService<ICartService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();
    LoadCartCommand = new RelayCommand(async () => await OnLoadCartAsync());
    AddToCartCommand = new RelayCommand<Guid>(async (productId) => await OnAddToCartAsync(productId));
    RemoveFromCartCommand = new RelayCommand<Guid>(async (cartItemId) => await OnRemoveFromCartAsync(cartItemId));
    ClearCartCommand = new RelayCommand(async () => await OnClearCartAsync());
  }

  private async Task OnLoadCartAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var userId = _sessionService.CurrentUser?.Id;
      if (userId == null)
      {
        ErrorMessage = "User not logged in";
        return;
      }
      var cart = await _cartService.GetUserCartAsync(userId.Value);
      if (cart != null)
      {
        CartItems = new ObservableCollection<CartItem>(cart.Items);
        CalculateTotals();
      }
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to load cart: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task OnAddToCartAsync(Guid productId)
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var userId = _sessionService.CurrentUser?.Id;
      if (userId == null)
      {
        ErrorMessage = "User not logged in";
        IsLoading = false;
        return;
      }
      await _cartService.AddToCartAsync(userId.Value, productId, 1);
      await OnLoadCartAsync();
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to add item: {ex.Message}";
      IsLoading = false;
    }
  }

  private async Task OnRemoveFromCartAsync(Guid cartItemId)
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var userId = _sessionService.CurrentUser?.Id;
      if (userId == null)
      {
        ErrorMessage = "User not logged in";
        IsLoading = false;
        return;
      }
      await _cartService.RemoveFromCartAsync(userId.Value, cartItemId);
      await OnLoadCartAsync();
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to remove item: {ex.Message}";
      IsLoading = false;
    }
  }

  private async Task OnClearCartAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var userId = _sessionService.CurrentUser?.Id;
      if (userId == null)
      {
        ErrorMessage = "User not logged in";
        IsLoading = false;
        return;
      }
      await _cartService.ClearCartAsync(userId.Value);
      CartItems.Clear();
      CalculateTotals();
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to clear cart: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private void CalculateTotals()
  {
    TotalPrice = CartItems.Sum(ci => ci.Quantity * ci.PriceAtAddTime);
    ItemCount = CartItems.Count;
  }
}