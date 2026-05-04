using floofy.Models.Enums;
using floofy.Services;

namespace floofy.ViewModels;

public class ProfileViewModel : BaseViewModel
{
  private readonly SessionService _sessionService;
  private readonly ICartService _cartService;

  private string _fullName = string.Empty;
  private string _email = string.Empty;
  private string _phoneNumber = string.Empty;
  private string _profileImage = string.Empty;
  private string _initials = "👤";
  private string _roleSummary = string.Empty;
  private bool _isAdopter;
  private bool _isRehomer;
  private int _cartItemCount;

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
    set
    {
      SetProperty(ref _phoneNumber, value);
      OnPropertyChanged(nameof(HasPhone));
    }
  }

  public bool HasPhone => !string.IsNullOrWhiteSpace(_phoneNumber);

  public string ProfileImage
  {
    get => _profileImage;
    set
    {
      SetProperty(ref _profileImage, value);
      OnPropertyChanged(nameof(HasProfileImage));
    }
  }

  public bool HasProfileImage => !string.IsNullOrWhiteSpace(_profileImage);

  public string Initials
  {
    get => _initials;
    set => SetProperty(ref _initials, value);
  }

  public string RoleSummary
  {
    get => _roleSummary;
    set => SetProperty(ref _roleSummary, value);
  }

  public bool IsAdopter
  {
    get => _isAdopter;
    set => SetProperty(ref _isAdopter, value);
  }

  public bool IsRehomer
  {
    get => _isRehomer;
    set => SetProperty(ref _isRehomer, value);
  }

  public int CartItemCount
  {
    get => _cartItemCount;
    set
    {
      SetProperty(ref _cartItemCount, value);
      OnPropertyChanged(nameof(HasCartItems));
      OnPropertyChanged(nameof(CartBadgeText));
    }
  }

  public bool HasCartItems => _cartItemCount > 0;
  public string CartBadgeText => _cartItemCount > 9 ? "9+" : _cartItemCount.ToString();

  public ProfileViewModel()
  {
    _sessionService = App.Services.GetRequiredService<SessionService>();
    _cartService = App.Services.GetRequiredService<ICartService>();
  }

  public void Refresh()
  {
    var user = _sessionService.CurrentUser;
    if (user is null)
    {
      FullName = "Not signed in";
      Email = "—";
      PhoneNumber = string.Empty;
      ProfileImage = string.Empty;
      Initials = "👤";
      RoleSummary = string.Empty;
      IsAdopter = false;
      IsRehomer = false;
      return;
    }

    FullName = string.IsNullOrWhiteSpace(user.FullName) ? "Floofy member" : user.FullName;
    Email = user.Email;
    PhoneNumber = user.PhoneNumber;
    ProfileImage = user.ProfileImageUrl;
    Initials = ComputeInitials(user.FullName);
    IsAdopter = user.IsBuyer;
    IsRehomer = user.IsSeller;

    RoleSummary = (IsAdopter, IsRehomer) switch
    {
      (true, true) => "Adopter • Rehomer",
      (true, false) => "Adopter",
      (false, true) => "Rehomer",
      _ => "Member"
    };
  }

  public async Task RefreshCartCountAsync()
  {
    var user = _sessionService.CurrentUser;
    if (user is null)
    {
      CartItemCount = 0;
      return;
    }

    try
    {
      var cart = await _cartService.GetUserCartAsync(user.Id);
      var products = cart.Items.Sum(i => i.Quantity);
      var pets = cart.PetItems.Count;
      CartItemCount = products + pets;
    }
    catch
    {
      CartItemCount = 0;
    }
  }

  private static string ComputeInitials(string fullName)
  {
    if (string.IsNullOrWhiteSpace(fullName)) return "👤";
    var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length == 0) return "👤";
    if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpperInvariant();
    return (parts[0].Substring(0, 1) + parts[^1].Substring(0, 1)).ToUpperInvariant();
  }
}
