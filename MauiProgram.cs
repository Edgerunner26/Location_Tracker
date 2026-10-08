using HeatMapApp.Services;
using HeatMapApp.ViewModels;
using HeatMapApp.Views;
using Microsoft.Extensions.Logging;

namespace HeatMapApp;

/// <summary>Configures the app, maps, and dependency injection.</summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps() // Enables the .NET MAUI Maps control.
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register services, view models, and pages so they can be injected.
        builder.Services.AddSingleton<LocationDatabase>();
        builder.Services.AddSingleton<LocationTrackingService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainView>();

        return builder.Build();
    }
}