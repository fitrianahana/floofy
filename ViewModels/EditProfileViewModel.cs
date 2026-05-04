using System.Windows.Input;
using floofy.Data;
using floofy.Models;
using floofy.Services;

namespace floofy.ViewModels;

public class EditProfileViewModel : BaseViewModel
{
  private readonly IRepository<User> _userRepository;
  private readonly SessionService _sessionService;

  private string _fullName = string.Empty;
  private string _email = string.Empty;
  private string _phoneNumber = string.Empty;
  private string _profileImageUrl = string.Empty;
  private string _statusMessage = string.Empty;

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

  public string PhoneNumber
  {
    get => _phoneNumber;
    set => SetProperty(ref _phoneNumber, value);
  }

  public string ProfileImageUrl
  {
    get => _profileImageUrl;
    set => SetProperty(ref _profileImageUrl, value);
  }

  public string StatusMessage
  {
    get => _statusMessage;
    set => SetProperty(ref _statusMessage, value);
  }

  public ICommand SaveCommand { get; }
  public Func<Task>? OnSaved { get; set; }

  public EditProfileViewModel()
  {
    _userRepository = App.Services.GetRequiredService<IRepository<User>>();
    _sessionService = App.Services.GetRequiredService<SessionService>();

    SaveCommand = new RelayCommand(async () => await OnSaveAsync());
  }

  public void LoadFromSession()
  {
    var user = _sessionService.CurrentUser;
    if (user is null) return;

    FullName = user.FullName;
    Email = user.Email;
    PhoneNumber = user.PhoneNumber;
    ProfileImageUrl = user.ProfileImageUrl;
    ErrorMessage = string.Empty;
    StatusMessage = string.Empty;
  }

  private async Task OnSaveAsync()
  {
    var user = _sessionService.CurrentUser;
    if (user is null)
    {
      ErrorMessage = "Please sign in again.";
      return;
    }

    var trimmedName = FullName?.Trim() ?? string.Empty;
    var trimmedEmail = Email?.Trim() ?? string.Empty;

    if (string.IsNullOrWhiteSpace(trimmedName))
    {
      ErrorMessage = "Full name cannot be empty.";
      return;
    }

    if (string.IsNullOrWhiteSpace(trimmedEmail) || !trimmedEmail.Contains('@'))
    {
      ErrorMessage = "Enter a valid email address.";
      return;
    }

    ErrorMessage = string.Empty;
    StatusMessage = string.Empty;
    IsLoading = true;

    try
    {
      user.FullName = trimmedName;
      user.Email = trimmedEmail;
      user.PhoneNumber = PhoneNumber?.Trim() ?? string.Empty;
      user.ProfileImageUrl = ProfileImageUrl?.Trim() ?? string.Empty;
      user.MarkAsUpdated();

      await _userRepository.UpdateAsync(user);
      _sessionService.SetCurrentUser(user);

      StatusMessage = "Profile updated.";

      if (OnSaved is not null)
      {
        await OnSaved();
      }
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Couldn't save: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }
}
