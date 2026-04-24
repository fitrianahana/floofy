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
  private bool _isBuyerSelected = false;
  private bool _isSellerSelected = false;

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

  public bool IsBuyerSelected
  {
    get => _isBuyerSelected;
    set => SetProperty(ref _isBuyerSelected, value);
  }

  public bool IsSellerSelected
  {
    get => _isSellerSelected;
    set => SetProperty(ref _isSellerSelected, value);
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
    Password == ConfirmPassword &&
    (IsBuyerSelected || IsSellerSelected);

  private async Task OnRegisterAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;

    try
    {
      var roles = GetSelectedRoles();
      if (roles.Count == 0)
      {
        ErrorMessage = "Please select at least one role";
        IsLoading = false;
        return;
      }

      var (success, message) = await _authService.RegisterAsync(new RegisterRequest
      {
        FullName = FullName.Trim(),
        Email = Email.Trim(),
        Password = Password,
        Roles = roles
      });

      if (!success)
      {
        ErrorMessage = message;
        return;
      }

      ErrorMessage = "Registration successful! Please login.";

      FullName = string.Empty;
      Email = string.Empty;
      Password = string.Empty;
      ConfirmPassword = string.Empty;
      IsBuyerSelected = false;
      IsSellerSelected = false;
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

  private List<RoleType> GetSelectedRoles()
  {
    var roles = new List<RoleType>();
    if (IsBuyerSelected) roles.Add(RoleType.Buyer);
    if (IsSellerSelected) roles.Add(RoleType.Seller);
    return roles;
  }

  private void OnGoToLogin()
  {
    // Navigate back to login - handled by View
  }
}