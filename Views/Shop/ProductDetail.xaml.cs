using floofy.ViewModels;

namespace floofy.Views;

[QueryProperty(nameof(ProductId), "productId")]
public partial class ProductDetail : ContentPage
{
  private readonly ProductDetailViewModel _viewModel;
  private Guid _productId = Guid.Empty;

  public string ProductId
  {
    set
    {
      if (Guid.TryParse(Uri.UnescapeDataString(value ?? string.Empty), out var parsed))
      {
        _productId = parsed;
      }
    }
  }

  public ProductDetail(ProductDetailViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    BindingContext = viewModel;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    if (_productId != Guid.Empty)
    {
      await _viewModel.LoadProductAsync(_productId);
    }
  }

  private async void OnBackClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("..");
  }
}
