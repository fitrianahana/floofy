using floofy.Services;
using floofy.Views;
using floofy.Data;

namespace floofy;

public partial class App : Application
{
  public static IServiceProvider Services { get; private set; } = default!;

  private readonly SessionService _sessionService;

  public App(IServiceProvider services, SessionService sessionService)
  {
    InitializeComponent();
    Services = services;
    _sessionService = sessionService;
    _ = InitializeDatabase();
  }

  private async Task InitializeDatabase()
  {
    try
    {
      var db = Services.GetRequiredService<AppDatabase>();
      await db.InitializeAsync();
      System.Diagnostics.Debug.WriteLine("Database initialized successfully");
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
    }
  }

  protected override Window CreateWindow(IActivationState? activationState)
  {
    if (_sessionService.IsLoggedIn)
      return new Window(Services.GetRequiredService<AppShell>());

    return new Window(new NavigationPage(Services.GetRequiredService<Login>()));
  }
}