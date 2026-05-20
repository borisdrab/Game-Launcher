using Launcher.DAL.Context;
using Launcher.DAL.Seeds;
using Launcher.DAL.Migrator;
using Launcher.DAL.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Launcher.DAL;

public static class DALInstaller
{
    public static IServiceCollection AddDALServices(this IServiceCollection services, string databaseName)
    {
        services.AddSingleton<IDbContextFactory<LauncherDbContext>>(_ =>
            new LauncherDbContextSqLiteFactory(databaseName));
        
        services.AddSingleton<IDbMigrator, DbMigrator>();
        services.AddSingleton<IDbSeeder, DbSeeder>();

        return services;
    }
}
