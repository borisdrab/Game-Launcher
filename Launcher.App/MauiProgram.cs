using CommunityToolkit.Maui;
using Launcher.App.Services;
using Launcher.BL;
using Launcher.DAL;
using Launcher.DAL.Context;
using Launcher.DAL.Seeds;
using Launcher.DAL.Migrator;
using Microsoft.EntityFrameworkCore;

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
            .AddDALServices("launcher.db")
            .AddBLServices()
            .AddAppServices();

        var app = builder.Build();

        MigrateDb(app.Services.GetRequiredService<IDbMigrator>());
        SeedDb(app.Services.GetRequiredService<IDbSeeder>());
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

    private static void MigrateDb(IDbMigrator migrator) => migrator.Migrate();

    private static void SeedDb(IDbSeeder dbSeeder) => dbSeeder.Seed();
}
