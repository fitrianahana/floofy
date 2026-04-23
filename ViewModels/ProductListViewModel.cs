using System.Collections.ObjectModel;
using floofy.Models;
using floofy.Services;
using System.Windows.Input;

namespace floofy.ViewModels;

public class ProductListViewModel : BaseViewModel
{
  private readonly IProductService _productService;
  private ObservableCollection<Product> _products = new();
  private string _searchQuery = string.Empty;
  private Product? _selectedProduct;

  public ObservableCollection<Product> Products
  {
    get => _products;
    set => SetProperty(ref _products, value);
  }

  public string SearchQuery
  {
    get => _searchQuery;
    set => SetProperty(ref _searchQuery, value);
  }

  public Product? SelectedProduct
  {
    get => _selectedProduct;
    set => SetProperty(ref _selectedProduct, value);
  }

  public ICommand LoadProductsCommand { get; }
  public ICommand SearchCommand { get; }

  public ProductListViewModel()
  {
    _productService = App.Services.GetRequiredService<IProductService>();
    LoadProductsCommand = new RelayCommand(async () => await OnLoadProductsAsync());
    SearchCommand = new RelayCommand(async () => await OnSearchAsync());
  }

  private async Task OnLoadProductsAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var products = await _productService.GetAllProductsAsync();
      Products = new ObservableCollection<Product>(products);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to load products: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task OnSearchAsync()
  {
    if (string.IsNullOrWhiteSpace(SearchQuery))
    {
      await OnLoadProductsAsync();
      return;
    }
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var products = await _productService.SearchProductsAsync(SearchQuery);
      Products = new ObservableCollection<Product>(products);
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