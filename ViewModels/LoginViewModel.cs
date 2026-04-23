using floofy.Models;
using floofy.Services;
using System.Windows.Input;

namespace floofy.ViewModels;

public class LoginViewModel : BaseViewModel
{
  private readonly IAuthService _authService;
  private readonly SessionService _sessionService;
  private string _email = string.Empty;
  private string _password = string.Empty;

  public string Email
  {
    get => _email;
    set => SetProperty(ref _email, value);
  }

  public string Password
  {
    get => _password;
    set => SetProperty(ref _password, value);
  }

  public ICommand LoginCommand { get; }

  public LoginViewModel()
  {
    _authService = App.Services.GetRequiredService<IAuthService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();
    LoginCommand = new RelayCommand(async () => await OnLoginAsync(), CanLogin);
  }

  private bool CanLogin() => !IsLoading && !string.IsNullOrWhiteSpace(Email);

  private async Task OnLoginAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var user = await _authService.LoginAsync(new LoginRequest
      {
        Email = Email.Trim(),
        Password = Password
      });
      if (user is null)
      {
        ErrorMessage = "Invalid email or password";
        return;
      }
      _sessionService.SetCurrentUser(user);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Login failed: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }
}