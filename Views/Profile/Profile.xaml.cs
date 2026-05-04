using floofy.Services;
using floofy.ViewModels;

namespace floofy.Views;

public partial class Profile : ContentPage
{
  private readonly SessionService _sessionService;
  private readonly ProfileViewModel _viewModel;

  public Profile(ProfileViewModel viewModel)
  {
    InitializeComponent();
    _sessionService = App.Services.GetRequiredService<SessionService>();
    _viewModel = viewModel;
    BindingContext = viewModel;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    _viewModel.Refresh();
    await _viewModel.RefreshCartCountAsync();
  }

  private async void OnEditProfileClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("editProfile");
  }

  private async void OnChangePasswordClicked(object? sender, EventArgs e)
  {
    await DisplayAlertAsync("Change Password", "Password changes will be available soon.", "OK");
  }

  private async void OnCartClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("cart");
  }

  private async void OnMyListingsClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("myListings");
  }

  private async void OnOrderHistoryClicked(object? sender, EventArgs e)
  {
    await DisplayAlertAsync("Order History", "Order history view is coming soon.", "OK");
  }

  private async void OnBookingsClicked(object? sender, EventArgs e)
  {
    await DisplayAlertAsync("My Bookings", "Booking history view is coming soon.", "OK");
  }

  private async void OnHelpClicked(object? sender, EventArgs e)
  {
    await DisplayAlertAsync("Help & Support", "Reach us at support@floofy.com — we typically reply within 24 hours.", "OK");
  }

  private async void OnAboutClicked(object? sender, EventArgs e)
  {
    await DisplayAlertAsync("About Floofy", "Floofy v1.0\nA caring marketplace for pets and the people who love them.", "OK");
  }

  private void OnLogoutClicked(object? sender, EventArgs e)
  {
    _sessionService.Logout();
    Application.Current!.Windows[0].Page = new NavigationPage(App.Services.GetRequiredService<Login>());
  }
}
