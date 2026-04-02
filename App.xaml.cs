using floofy.Services;
using floofy.Views;

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
  }

  protected override Window CreateWindow(IActivationState? activationState)
  {
    if (_sessionService.IsLoggedIn)
      return new Window(Services.GetRequiredService<AppShell>());

    return new Window(new NavigationPage(Services.GetRequiredService<Login>()));
  }
}