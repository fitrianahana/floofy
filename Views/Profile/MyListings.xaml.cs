using floofy.Models;
using floofy.ViewModels;

namespace floofy.Views;

public partial class MyListings : ContentPage
{
  private readonly MyListingsViewModel _viewModel;

  public MyListings(MyListingsViewModel viewModel)
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

  private async void OnBackClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("..");
  }

  private async void OnListingTapped(object? sender, EventArgs e)
  {
    if (sender is Element el && el.BindingContext is Pet pet)
    {
      await Shell.Current.GoToAsync($"petDetail?petId={pet.Id}");
    }
  }

  private async void OnRehomeClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("sellPet");
  }

  private async void OnCancelListingClicked(object? sender, EventArgs e)
  {
    if (sender is not Element el || el.BindingContext is not Pet pet) return;

    var dialog = new CancelListingDialog(pet.Name);
    await Navigation.PushModalAsync(dialog);
    var confirmed = await dialog.WaitForResultAsync();

    if (confirmed)
    {
      var name = pet.Name;
      await _viewModel.CancelListingAsync(pet);
      _ = ShowToastAsync($"{name}'s listing was cancelled");
    }
  }

  private async Task ShowToastAsync(string message)
  {
    ToastMessage.Text = message;
    Toast.Opacity = 0;
    Toast.IsVisible = true;
    await Toast.FadeToAsync(1, 200, Easing.CubicOut);
    await Task.Delay(2200);
    await Toast.FadeToAsync(0, 250, Easing.CubicIn);
    Toast.IsVisible = false;
  }
}
