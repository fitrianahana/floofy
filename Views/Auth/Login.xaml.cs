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
    viewModel.PropertyChanged += (s, e) =>
    {
      if (e.PropertyName == nameof(LoginViewModel.IsLoading) && !viewModel.IsLoading)
      {
        if (string.IsNullOrEmpty(viewModel.ErrorMessage))
        {
          if (Application.Current?.Windows.FirstOrDefault() is Window window)
          {
            window.Page = App.Services.GetRequiredService<AppShell>();
          }
        }
      }
    };
  }

  private async void OnGoToRegisterClicked(object sender, EventArgs e)
  {
    await Navigation.PushAsync(App.Services.GetRequiredService<Register>());
  }
}