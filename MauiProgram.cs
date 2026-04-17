using Microsoft.Extensions.Logging;
using floofy.Data;
using floofy.Services;
using floofy.Views;

namespace floofy
{
  public static class MauiProgram
  {
    public static MauiApp CreateMauiApp()
    {
      var builder = MauiApp.CreateBuilder();
      builder
          .UseMauiApp<App>()
          .ConfigureFonts(fonts =>
          {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
          });

      // Data layer
      builder.Services.AddSingleton<AppDatabase>();
      builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

      // Core services
      builder.Services.AddScoped<IOrderService, OrderService>();
      builder.Services.AddScoped<IProductService, ProductService>();
      builder.Services.AddScoped<ICartService, CartService>();

      // Register services
      builder.Services.AddSingleton<SessionService>();
      builder.Services.AddSingleton<IAuthService, MockAuthService>();

      // Register pages
      builder.Services.AddSingleton<AppShell>();
      builder.Services.AddTransient<Login>();
      builder.Services.AddTransient<Register>();

#if DEBUG
      builder.Logging.AddDebug();
#endif

      return builder.Build();
    }
  }
}
