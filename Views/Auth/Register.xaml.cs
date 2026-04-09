using floofy.Models;
using floofy.Services;
namespace floofy.Views;

public partial class Register : ContentPage
{
  private readonly IAuthService _authService;
  public Register()
  {
    InitializeComponent();
    _authService = App.Services.GetRequiredService<IAuthService>();
  }

  private async void OnRegisterClicked(object? sender, EventArgs e)
  {
    var fullName = FullNameEntry.Text?.Trim() ?? string.Empty;
    var email = EmailEntry.Text?.Trim() ?? string.Empty;
    var password = PasswordEntry.Text ?? string.Empty;

    // Validation
    if (string.IsNullOrWhiteSpace(fullName) ||
        string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(password))
    {
      await DisplayAlertAsync("Validation", "All fields are required.", "OK");
      return;
    }

    // Collect selected roles
    var roles = new List<RoleType>();
    if (BuyerCheckBox.IsChecked) roles.Add(RoleType.Buyer);
    if (SellerCheckBox.IsChecked) roles.Add(RoleType.Seller);
    // Validation: at least one role selected
    if (roles.Count == 0)
    {
      await DisplayAlertAsync("Validation", "Please select at least one role.", "OK");
      return;
    }

    // Call service to register
    var result = await _authService.RegisterAsync(new RegisterRequest
    {
      FullName = fullName,
      Email = email,
      Password = password,
      Roles = roles
    });

    // Check result
    if (!result.Success)
    {
      await DisplayAlertAsync("Registration Failed", result.Message, "OK");
      return;
    }

    // Success
    await DisplayAlertAsync("Success", "Account created! Please login.", "OK");
    await Navigation.PopAsync();  // Return to login page
  }

  private async void OnBackToLoginClicked(object? sender, EventArgs e)
  {
    await Navigation.PopAsync();  // Return to login page
  }
}
