using System.Diagnostics;
using floofy.Services;
using floofy.Views;
using floofy.Data;

namespace floofy;

public partial class App : Application
{
  public static IServiceProvider Services { get; private set; } = default!;

  private readonly SessionService _sessionService;
  private bool _isStartupCompleted;

  public App(IServiceProvider services, SessionService sessionService)
  {
    InitializeComponent();
    Services = services;
    _sessionService = sessionService;
  }

  protected override Window CreateWindow(IActivationState? activationState)
  {
    // Temporary loading page while DB initializes
    var loadingPage = new ContentPage
    {
      Content = new Grid
      {
        Children =
        {
          new ActivityIndicator
          {
            IsRunning = true,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
          }
        }
      }
    };

    var window = new Window(loadingPage);

    _ = InitializeAndNavigateAsync(window);

    return window;
  }

  private async Task InitializeAndNavigateAsync(Window window)
  {
    if (_isStartupCompleted)
    {
      window.Page = _sessionService.IsLoggedIn
        ? Services.GetRequiredService<AppShell>()
        : new NavigationPage(Services.GetRequiredService<Login>());
      return;
    }

    try
    {
      var db = Services.GetRequiredService<AppDatabase>();
      await db.InitializeAsync();
      await db.SeedDataIfEmptyAsync();
      _isStartupCompleted = true;
      Debug.WriteLine("Startup initialization completed");
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Startup initialization failed: {ex.Message}");
    }

    window.Page = _sessionService.IsLoggedIn
      ? Services.GetRequiredService<AppShell>()
      : new NavigationPage(Services.GetRequiredService<Login>());
  }
}