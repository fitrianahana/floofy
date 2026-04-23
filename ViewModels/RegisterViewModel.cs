using floofy.Models;
using floofy.Models.Enums;
using floofy.Services;
using System.Windows.Input;
namespace floofy.ViewModels;

public class RegisterViewModel : BaseViewModel
{
  private readonly IAuthService _authService;
  private readonly SessionService _sessionService;
  private string _fullName = string.Empty;
  private string _email = string.Empty;
  private string _password = string.Empty;
  private string _confirmPassword = string.Empty;

  public string FullName
  {
    get => _fullName;
    set => SetProperty(ref _fullName, value);
  }

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

  public string ConfirmPassword
  {
    get => _confirmPassword;
    set => SetProperty(ref _confirmPassword, value);
  }

  public ICommand RegisterCommand { get; }
  public ICommand GoToLoginCommand { get; }

  public RegisterViewModel()
  {
    _authService = App.Services.GetRequiredService<IAuthService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();
    RegisterCommand = new RelayCommand(async () => await OnRegisterAsync(), CanRegister);
    GoToLoginCommand = new RelayCommand(OnGoToLogin);
  }

  private bool CanRegister() =>
    !IsLoading &&
    !string.IsNullOrWhiteSpace(FullName) &&
    !string.IsNullOrWhiteSpace(Email) &&
    !string.IsNullOrWhiteSpace(Password) &&
    Password == ConfirmPassword;
  private async Task OnRegisterAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var (success, message) = await _authService.RegisterAsync(new RegisterRequest
      {
        FullName = FullName.Trim(),
        Email = Email.Trim(),
        Password = Password,
        Roles = new List<RoleType> { RoleType.Buyer } // Default role
      });
      if (!success)
      {
        ErrorMessage = message;
        return;
      }
      ErrorMessage = "Registration successful! Please login.";
      // Could auto-navigate to login here or let user do it manually
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Registration failed: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }
  private void OnGoToLogin()
  {
    // Navigate back to login - handled by View
  }
}