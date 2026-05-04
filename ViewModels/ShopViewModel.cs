using System.Collections.ObjectModel;
using System.Windows.Input;
using floofy.Models;
using floofy.Services;

namespace floofy.ViewModels;

public enum ShopSection
{
  Pets,
  Products
}

public class ShopViewModel : BaseViewModel
{
  private readonly IPetService _petService;
  private readonly IProductService _productService;
  private readonly SessionService _sessionService;
  private ShopSection _activeSection = ShopSection.Pets;
  private string _searchQuery = string.Empty;
  private ObservableCollection<Pet> _pets = new();
  private ObservableCollection<Product> _products = new();

  public ShopSection ActiveSection
  {
    get => _activeSection;
    set
    {
      SetProperty(ref _activeSection, value);
      OnPropertyChanged(nameof(IsPetsSectionActive));
      OnPropertyChanged(nameof(IsProductsSectionActive));
      OnPropertyChanged(nameof(ShowPetsList));
      OnPropertyChanged(nameof(ShowProductsList));
      OnPropertyChanged(nameof(CanShowSellAction));
      OnPropertyChanged(nameof(SearchPlaceholder));
      OnPropertyChanged(nameof(HeaderSubtitle));
      OnPropertyChanged(nameof(HeaderBackground));
    }
  }

  public bool IsPetsSectionActive => ActiveSection == ShopSection.Pets;
  public bool IsProductsSectionActive => ActiveSection == ShopSection.Products;

  public bool ShowPetsList => IsPetsSectionActive && !IsLoading;
  public bool ShowProductsList => IsProductsSectionActive && !IsLoading;

  public bool IsBuyer => _sessionService.CurrentUser?.IsBuyer ?? false;
  public bool IsSeller => _sessionService.CurrentUser?.IsSeller ?? false;
  public bool CanShowSellAction => IsSeller && IsPetsSectionActive;
  public string HeaderSubtitle => IsPetsSectionActive
    ? "Find your new best friend"
    : "Browse and discover amazing products";

  public string SearchPlaceholder => IsPetsSectionActive ? "Search pets" : "Search products";

  public string HeaderBackground => IsPetsSectionActive ? "petshop_bg.jpg" : "shop_bg.jpg";

  public string SearchQuery
  {
    get => _searchQuery;
    set => SetProperty(ref _searchQuery, value);
  }

  public ObservableCollection<Pet> Pets
  {
    get => _pets;
    set => SetProperty(ref _pets, value);
  }

  public ObservableCollection<Product> Products
  {
    get => _products;
    set => SetProperty(ref _products, value);
  }

  public ICommand SwitchToPetsCommand { get; }
  public ICommand SwitchToProductsCommand { get; }
  public ICommand LoadCurrentSectionCommand { get; }
  public ICommand SearchCommand { get; }

  public ShopViewModel()
  {
    _petService = App.Services.GetRequiredService<IPetService>();
    _productService = App.Services.GetRequiredService<IProductService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();

    PropertyChanged += (_, e) =>
    {
      if (e.PropertyName == nameof(IsLoading))
      {
        OnPropertyChanged(nameof(ShowPetsList));
        OnPropertyChanged(nameof(ShowProductsList));
      }
    };

    SwitchToPetsCommand = new RelayCommand(async () =>
    {
      ActiveSection = ShopSection.Pets;
      await LoadCurrentSectionAsync();
    });

    SwitchToProductsCommand = new RelayCommand(async () =>
    {
      ActiveSection = ShopSection.Products;
      await LoadCurrentSectionAsync();
    });

    LoadCurrentSectionCommand = new RelayCommand(async () => await LoadCurrentSectionAsync());
    SearchCommand = new RelayCommand(async () => await SearchCurrentSectionAsync());
  }

  public void RefreshRoles()
  {
    OnPropertyChanged(nameof(IsBuyer));
    OnPropertyChanged(nameof(IsSeller));
    OnPropertyChanged(nameof(CanShowSellAction));
    OnPropertyChanged(nameof(HeaderSubtitle));
  }

  private async Task LoadCurrentSectionAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;

    try
    {
      if (ActiveSection == ShopSection.Pets)
      {
        var pets = await _petService.GetAllPetsAsync();
        Pets = new ObservableCollection<Pet>(pets);
      }
      else
      {
        var products = await _productService.GetAllProductsAsync();
        Products = new ObservableCollection<Product>(products);
      }
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to load data: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task SearchCurrentSectionAsync()
  {
    if (string.IsNullOrWhiteSpace(SearchQuery))
    {
      await LoadCurrentSectionAsync();
      return;
    }

    ErrorMessage = string.Empty;
    IsLoading = true;

    try
    {
      if (ActiveSection == ShopSection.Pets)
      {
        var pets = await _petService.SearchPetsAsync(SearchQuery.Trim());
        Pets = new ObservableCollection<Pet>(pets);
      }
      else
      {
        var products = await _productService.SearchProductsAsync(SearchQuery.Trim());
        Products = new ObservableCollection<Product>(products);
      }
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Search failed: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }
}