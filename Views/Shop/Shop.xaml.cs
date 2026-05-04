using floofy.Models;
using floofy.ViewModels;
using System.Windows.Input;

namespace floofy.Views;

public partial class Shop : ContentPage
{
  private readonly ShopViewModel _viewModel;
  private bool _isPetsTabActive = true;

  public Shop(ShopViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    BindingContext = viewModel;
  }

  protected override void OnAppearing()
  {
    base.OnAppearing();
    _viewModel.RefreshRoles();
    if (_viewModel.LoadCurrentSectionCommand is ICommand cmd && cmd.CanExecute(null))
    {
      cmd.Execute(null);
    }
  }

  private void OnPetsTabClicked(object? sender, EventArgs e)
  {
    _isPetsTabActive = true;
    UpdateTabUI();
    if (_viewModel.SwitchToPetsCommand is ICommand cmd && cmd.CanExecute(null))
    {
      cmd.Execute(null);
    }
  }

  private void OnProductsTabClicked(object? sender, EventArgs e)
  {
    _isPetsTabActive = false;
    UpdateTabUI();
    if (_viewModel.SwitchToProductsCommand is ICommand cmd && cmd.CanExecute(null))
    {
      cmd.Execute(null);
    }
  }

  private void UpdateTabUI()
  {
    if (_isPetsTabActive)
    {
      PetsTabButton.TextColor = Color.FromArgb("#2D1B4E");
      PetsTabButton.FontAttributes = FontAttributes.Bold;
      ProductsTabButton.TextColor = Color.FromArgb("#6B5B8C");
      ProductsTabButton.FontAttributes = FontAttributes.None;
      MainThread.BeginInvokeOnMainThread(async () =>
      {
        await TabIndicator.TranslateToAsync(0, 0, 200, Easing.Linear);
      });
    }
    else
    {
      PetsTabButton.TextColor = Color.FromArgb("#6B5B8C");
      PetsTabButton.FontAttributes = FontAttributes.None;
      ProductsTabButton.TextColor = Color.FromArgb("#2D1B4E");
      ProductsTabButton.FontAttributes = FontAttributes.Bold;
      // Animate indicator to Products tab position (Pets width ~58 + spacing 24 = ~82px)
      MainThread.BeginInvokeOnMainThread(async () =>
      {
        await TabIndicator.TranslateToAsync(82, 0, 200, Easing.Linear);
      });
    }
  }

  private void OnSearchButtonPressed(object sender, EventArgs e)
  {
    if (_viewModel.SearchCommand is ICommand cmd && cmd.CanExecute(null))
    {
      cmd.Execute(null);
    }
  }

  private async void OnPetCardTapped(object? sender, EventArgs e)
  {
    if (sender is Element element && element.BindingContext is Pet pet)
    {
      await Shell.Current.GoToAsync($"petDetail?petId={pet.Id}");
    }
  }

  private async void OnProductCardTapped(object? sender, EventArgs e)
  {
    if (sender is Element element && element.BindingContext is Product product)
    {
      await Shell.Current.GoToAsync($"productDetail?productId={product.Id}");
    }
  }

  private async void OnCartClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("cart");
  }

  private async void OnSellClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("sellPet");
  }
}
