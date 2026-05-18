using CommunityToolkit.Maui;
using Launcher.App.Services;
using Launcher.BL;
using Launcher.DAL;
using Launcher.DAL.Context;
using Microsoft.EntityFrameworkCore;

using System.IO;

namespace Launcher.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services
            //.AddDALServices("launcher.db") comment for my MacCatalyst error
            .AddDALServices(Path.Combine(FileSystem.AppDataDirectory, "launcher.db"))
            .AddBLServices()
            .AddAppServices();

        var app = builder.Build();

        MigrateDb(app);
        RegisterRouting(app.Services.GetRequiredService<INavigationService>());

        return app;
    }

    private static void RegisterRouting(INavigationService navigationService)
    {
        foreach (var route in navigationService.Routes)
        {
            Routing.RegisterRoute(route.Route, route.ViewType);
        }
    }

    private static void MigrateDb(MauiApp app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LauncherDbContext>();
        dbContext.Database.Migrate();
    }
}
