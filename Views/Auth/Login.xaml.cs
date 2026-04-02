using floofy.Models;
using floofy.Services;
namespace floofy.Views;

public partial class Login : ContentPage
{
  private readonly IAuthService _authService;
  private readonly SessionService _sessionService;

  public Login()
  {
    InitializeComponent();
    _authService = App.Services.GetRequiredService<IAuthService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();
  }
  private async void OnLoginClicked(object? sender, EventArgs e)
  {
    var email = EmailEntry.Text?.Trim() ?? string.Empty;
    var password = PasswordEntry.Text ?? string.Empty;
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
      await DisplayAlertAsync("Validation", "Email and password are required.", "OK");
      return;
    }
    var user = await _authService.LoginAsync(new LoginRequest
    {
      Email = email,
      Password = password
    });
    if (user is null)
    {
      await DisplayAlertAsync("Login Failed", "Invalid email or password.", "OK");
      return;
    }
    _sessionService.SetCurrentUser(user);
    Application.Current!.Windows[0].Page = App.Services.GetRequiredService<AppShell>();
  }
  private async void OnGoToRegisterClicked(object? sender, EventArgs e)
  {
    await Navigation.PushAsync(App.Services.GetRequiredService<Register>());
  }
}