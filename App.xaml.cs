using System.Diagnostics;
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
    _ = SeedDatabaseAsync();
  }

  private async Task InitializeDatabase()
  {
    try
    {
      var db = Services.GetRequiredService<AppDatabase>();
      await db.InitializeAsync();
      Debug.WriteLine("Database initialized successfully");
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Database initialization failed: {ex.Message}");
    }
  }

  private async Task SeedDatabaseAsync()
  {
    try
    {
      var appDatabase = this.Handler?.MauiContext?.Services.GetService<AppDatabase>();
      if (appDatabase != null)
      {
        await appDatabase.SeedDataIfEmptyAsync();
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error seeding database: {ex.Message}");
    }
  }

  protected override Window CreateWindow(IActivationState? activationState)
  {
    if (_sessionService.IsLoggedIn)
      return new Window(Services.GetRequiredService<AppShell>());

    return new Window(new NavigationPage(Services.GetRequiredService<Login>()));
  }
}