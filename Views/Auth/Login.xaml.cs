using floofy.ViewModels;

namespace floofy.Views;

public partial class Login : ContentPage
{
  private readonly LoginViewModel _viewModel;
  public Login(LoginViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    BindingContext = viewModel;
    // Handle navigation after successful login
    viewModel.PropertyChanged += async (s, e) =>
    {
      if (e.PropertyName == nameof(LoginViewModel.IsLoading) && !viewModel.IsLoading)
      {
        // Login attempt completed (IsLoading changed from true to false)
        // Check if there's an error message
        if (string.IsNullOrEmpty(viewModel.ErrorMessage))
        {
          // Success! No error message means login succeeded
          await Navigation.PushAsync(App.Services.GetRequiredService<Home>());
        }
      }
    };
  }
  private async void OnGoToRegisterClicked(object sender, EventArgs e)
  {
    await Navigation.PushAsync(App.Services.GetRequiredService<Register>());
  }
}