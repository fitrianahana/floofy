using System.Collections.ObjectModel;
using System.Collections.Specialized;
using floofy.Models;
using floofy.Services;
using System.Windows.Input;

namespace floofy.ViewModels;

public class CartViewModel : BaseViewModel
{
  private readonly ICartService _cartService;
  private readonly IProductService _productService;
  private readonly IPetService _petService;
  private readonly SessionService _sessionService;

  private readonly ObservableCollection<CartLineItem> _items = new();
  private readonly ObservableCollection<PetCartLineItem> _petItems = new();
  private decimal _totalPrice;
  private int _itemCount;
  private string _statusMessage = string.Empty;

  public ObservableCollection<CartLineItem> Items => _items;
  public ObservableCollection<PetCartLineItem> PetItems => _petItems;

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

  public string StatusMessage
  {
    get => _statusMessage;
    set => SetProperty(ref _statusMessage, value);
  }

  public bool HasProducts => Items.Count > 0;
  public bool HasPets => PetItems.Count > 0;
  public bool IsEmpty => !HasProducts && !HasPets;
  public bool ShowList => !IsLoading && !IsEmpty;
  public bool ShowEmpty => !IsLoading && IsEmpty;

  public ICommand LoadCartCommand { get; }
  public ICommand IncrementCommand { get; }
  public ICommand DecrementCommand { get; }
  public ICommand RemoveCommand { get; }
  public ICommand RemovePetCommand { get; }
  public ICommand ClearCartCommand { get; }
  public ICommand CheckoutCommand { get; }

  public CartViewModel()
  {
    _cartService = App.Services.GetRequiredService<ICartService>();
    _productService = App.Services.GetRequiredService<IProductService>();
    _petService = App.Services.GetRequiredService<IPetService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();

    Items.CollectionChanged += OnAnyCollectionChanged;
    PetItems.CollectionChanged += OnAnyCollectionChanged;

    PropertyChanged += (_, e) =>
    {
      if (e.PropertyName == nameof(IsLoading))
      {
        OnPropertyChanged(nameof(ShowList));
        OnPropertyChanged(nameof(ShowEmpty));
      }
    };

    LoadCartCommand = new RelayCommand(async () => await LoadAsync());
    IncrementCommand = new RelayCommand<CartLineItem>(async (line) => await ChangeQuantityAsync(line, +1));
    DecrementCommand = new RelayCommand<CartLineItem>(async (line) => await ChangeQuantityAsync(line, -1));
    RemoveCommand = new RelayCommand<CartLineItem>(async (line) => await RemoveAsync(line));
    RemovePetCommand = new RelayCommand<PetCartLineItem>(async (line) => await RemovePetAsync(line));
    ClearCartCommand = new RelayCommand(async () => await ClearAsync());
    CheckoutCommand = new RelayCommand(OnCheckout);
  }

  private void OnAnyCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    RecalculateTotals();
    OnPropertyChanged(nameof(IsEmpty));
    OnPropertyChanged(nameof(HasProducts));
    OnPropertyChanged(nameof(HasPets));
    OnPropertyChanged(nameof(ShowList));
    OnPropertyChanged(nameof(ShowEmpty));
  }

  public async Task LoadAsync()
  {
    var user = _sessionService.CurrentUser;
    if (user is null)
    {
      ErrorMessage = "Please sign in to view your cart.";
      Items.Clear();
      PetItems.Clear();
      return;
    }

    ErrorMessage = string.Empty;
    StatusMessage = string.Empty;
    IsLoading = true;

    try
    {
      var cart = await _cartService.GetUserCartAsync(user.Id);
      Items.Clear();
      PetItems.Clear();

      foreach (var ci in cart.Items)
      {
        var product = await _productService.GetProductByIdAsync(ci.ProductId);
        Items.Add(new CartLineItem
        {
          CartItemId = ci.Id,
          ProductId = ci.ProductId,
          ProductName = product?.Name ?? "Unknown product",
          Thumbnail = product?.Thumbnail ?? string.Empty,
          UnitPrice = ci.PriceAtAddTime,
          Quantity = ci.Quantity
        });
      }

      foreach (var pi in cart.PetItems)
      {
        var pet = await _petService.GetPetByIdAsync(pi.PetId);
        PetItems.Add(new PetCartLineItem
        {
          PetCartItemId = pi.Id,
          PetId = pi.PetId,
          PetName = pet?.Name ?? "Unknown pet",
          Thumbnail = pet?.Thumbnail ?? string.Empty,
          Species = pet?.Species ?? string.Empty,
          Breed = pet?.Breed ?? string.Empty,
          AdoptionFee = pi.AdoptionFee
        });
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

  private async Task ChangeQuantityAsync(CartLineItem? line, int delta)
  {
    if (line is null) return;
    var newQty = line.Quantity + delta;
    if (newQty <= 0)
    {
      await RemoveAsync(line);
      return;
    }

    try
    {
      await _cartService.UpdateCartItemQuantityAsync(line.CartItemId, newQty);
      line.Quantity = newQty;
      RecalculateTotals();
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Could not update quantity: {ex.Message}";
    }
  }

  private async Task RemoveAsync(CartLineItem? line)
  {
    if (line is null) return;
    var user = _sessionService.CurrentUser;
    if (user is null) return;

    try
    {
      await _cartService.RemoveFromCartAsync(user.Id, line.CartItemId);
      Items.Remove(line);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Could not remove item: {ex.Message}";
    }
  }

  private async Task RemovePetAsync(PetCartLineItem? line)
  {
    if (line is null) return;
    var user = _sessionService.CurrentUser;
    if (user is null) return;

    try
    {
      await _cartService.RemovePetFromCartAsync(user.Id, line.PetCartItemId);
      PetItems.Remove(line);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Could not remove pet: {ex.Message}";
    }
  }

  private async Task ClearAsync()
  {
    var user = _sessionService.CurrentUser;
    if (user is null) return;

    try
    {
      await _cartService.ClearCartAsync(user.Id);
      Items.Clear();
      PetItems.Clear();
      StatusMessage = "Cart cleared.";
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Could not clear cart: {ex.Message}";
    }
  }

  private void OnCheckout()
  {
    if (IsEmpty) return;
    StatusMessage = "Checkout flow is coming soon!";
  }

  private void RecalculateTotals()
  {
    decimal total = 0m;
    int count = 0;
    foreach (var item in Items)
    {
      total += item.Subtotal;
      count += item.Quantity;
    }
    foreach (var pet in PetItems)
    {
      total += pet.AdoptionFee;
      count += 1;
    }
    TotalPrice = total;
    ItemCount = count;
  }
}
