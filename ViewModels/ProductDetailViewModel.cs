using System.Windows.Input;
using floofy.Models;
using floofy.Services;

namespace floofy.ViewModels;

public class ProductDetailViewModel : BaseViewModel
{
  private readonly IProductService _productService;
  private readonly ICartService _cartService;
  private readonly SessionService _sessionService;

  private Product? _product;
  private int _quantity = 1;
  private string _statusMessage = string.Empty;
  private bool _isAddingToCart;

  public Product? Product
  {
    get => _product;
    set
    {
      SetProperty(ref _product, value);
      OnPropertyChanged(nameof(LineTotal));
      OnPropertyChanged(nameof(CanAddToCart));
    }
  }

  public int Quantity
  {
    get => _quantity;
    set
    {
      var clamped = value;
      if (clamped < 1) clamped = 1;
      var max = Product?.StockQuantity ?? 1;
      if (max < 1) max = 1;
      if (clamped > max) clamped = max;
      SetProperty(ref _quantity, clamped);
      OnPropertyChanged(nameof(LineTotal));
    }
  }

  public decimal LineTotal => (Product?.DiscountedPrice ?? 0m) * Quantity;

  public bool CanAddToCart => Product is not null && Product.InStock && !_isAddingToCart;

  public bool IsAddingToCart
  {
    get => _isAddingToCart;
    set
    {
      SetProperty(ref _isAddingToCart, value);
      OnPropertyChanged(nameof(CanAddToCart));
    }
  }

  public string StatusMessage
  {
    get => _statusMessage;
    set => SetProperty(ref _statusMessage, value);
  }

  public ICommand IncrementQuantityCommand { get; }
  public ICommand DecrementQuantityCommand { get; }
  public ICommand AddToCartCommand { get; }

  public ProductDetailViewModel()
  {
    _productService = App.Services.GetRequiredService<IProductService>();
    _cartService = App.Services.GetRequiredService<ICartService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();

    IncrementQuantityCommand = new RelayCommand(() => Quantity += 1);
    DecrementQuantityCommand = new RelayCommand(() => Quantity -= 1);
    AddToCartCommand = new RelayCommand(async () => await OnAddToCartAsync());
  }

  public async Task LoadProductAsync(Guid productId)
  {
    ErrorMessage = string.Empty;
    StatusMessage = string.Empty;
    IsLoading = true;

    try
    {
      var product = await _productService.GetProductByIdAsync(productId);
      if (product == null)
      {
        ErrorMessage = "Product not found.";
        Product = null;
      }
      else
      {
        Product = product;
        Quantity = product.InStock ? 1 : 0;
      }
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to load product: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task OnAddToCartAsync()
  {
    if (Product is null || !Product.InStock) return;

    var user = _sessionService.CurrentUser;
    if (user is null)
    {
      ErrorMessage = "Please sign in to add items to your cart.";
      return;
    }

    ErrorMessage = string.Empty;
    StatusMessage = string.Empty;
    IsAddingToCart = true;

    try
    {
      await _cartService.AddToCartAsync(user.Id, Product.Id, Quantity);
      StatusMessage = $"Added {Quantity} × {Product.Name} to your cart.";
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Could not add to cart: {ex.Message}";
    }
    finally
    {
      IsAddingToCart = false;
    }
  }
}
