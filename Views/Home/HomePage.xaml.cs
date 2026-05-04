using floofy.Models;
using floofy.ViewModels;

namespace floofy.Views;

public partial class Home : ContentPage
{
  private readonly HomeViewModel _viewModel;

  public Home(HomeViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    BindingContext = viewModel;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    await _viewModel.LoadAsync();
  }

  private async void OnCartTapped(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("cart");
  }

  private async void OnSearchTapped(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("//shop");
  }

  private async void OnFindPetsTapped(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("//shop");
  }

  private async void OnProductsTapped(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("//shop");
  }

  private async void OnCommunityTapped(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("//community");
  }

  private async void OnVetTapped(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("//plus");
  }

  private async void OnRehomeTapped(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("sellPet");
  }

  private async void OnFeaturedPetTapped(object? sender, EventArgs e)
  {
    if (sender is Element el && el.BindingContext is Pet pet)
    {
      await Shell.Current.GoToAsync($"petDetail?petId={pet.Id}");
    }
  }

  private async void OnFeaturedProductTapped(object? sender, EventArgs e)
  {
    if (sender is Element el && el.BindingContext is Product product)
    {
      await Shell.Current.GoToAsync($"productDetail?productId={product.Id}");
    }
  }

  private async void OnArticleTapped(object? sender, EventArgs e)
  {
    if (sender is Element el && el.BindingContext is HomeArticle article)
    {
      await DisplayAlertAsync(article.Title, $"{article.Excerpt}\n\nFull article reader coming soon.", "Close");
    }
  }
}
