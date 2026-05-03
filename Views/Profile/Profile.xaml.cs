using floofy.Services;
namespace floofy.Views;

public partial class Profile : ContentPage
{
  private readonly SessionService _sessionService;
  public Profile()
  {
    InitializeComponent();
    _sessionService = App.Services.GetRequiredService<SessionService>();
  }
  protected override void OnAppearing()
  {
    base.OnAppearing();
    LoadUserInfo();
  }
  private void LoadUserInfo()
  {
    var currentUser = _sessionService.CurrentUser;
    if (currentUser is null)
    {
      FullNameLabel.Text = "Not Signed In";
      EmailLabel.Text = "N/A";
      RolesLabel.Text = "N/A";
      return;
    }
    FullNameLabel.Text = currentUser.FullName;
    EmailLabel.Text = currentUser.Email;
    RolesLabel.Text = string.Join(", ", currentUser.Roles);
  }
  private async void OnLogoutClicked(object? sender, EventArgs e)
  {
    _sessionService.Logout();
    Application.Current!.Windows[0].Page = new NavigationPage(App.Services.GetRequiredService<Login>());
  }
}