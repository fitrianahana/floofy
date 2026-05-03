using Microsoft.Extensions.Logging;
using floofy.Data;
using floofy.Services;
using floofy.Views;
using floofy.ViewModels;
using floofy.Converters;

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

      // Extended services
      builder.Services.AddScoped<IBookingService, BookingService>();
      builder.Services.AddScoped<IPetService, PetService>();
      builder.Services.AddScoped<ICommunityService, CommunityService>();
      builder.Services.AddScoped<IPaymentService, PaymentService>();
      builder.Services.AddScoped<IReportService, ReportService>();

      // ViewModels - MVVM layer
      builder.Services.AddTransient<LoginViewModel>();
      builder.Services.AddTransient<RegisterViewModel>();
      builder.Services.AddTransient<ProductListViewModel>();
      builder.Services.AddTransient<CartViewModel>();
      builder.Services.AddTransient<OrderViewModel>();
      builder.Services.AddTransient<PetListViewModel>();
      builder.Services.AddTransient<BookingViewModel>();
      builder.Services.AddTransient<CommunityViewModel>();
      builder.Services.AddTransient<ShopViewModel>();
      builder.Services.AddTransient<PetDetailViewModel>();
      builder.Services.AddTransient<ProductDetailViewModel>();

      // Register services
      builder.Services.AddSingleton<SessionService>();
      builder.Services.AddScoped<IAuthService, AuthService>();

      // Register pages
      builder.Services.AddSingleton<AppShell>();
      builder.Services.AddTransient<Login>();
      builder.Services.AddTransient<Register>();
      builder.Services.AddTransient<Home>();
      builder.Services.AddTransient<Shop>();
      builder.Services.AddTransient<Community>();
      builder.Services.AddTransient<FloofyPlus>();
      builder.Services.AddTransient<Profile>();
      builder.Services.AddTransient<PetDetail>();
      builder.Services.AddTransient<ProductDetail>();

#if DEBUG
      builder.Logging.AddDebug();
#endif

      var app = builder.Build();
      return app;
    }
  }
}
