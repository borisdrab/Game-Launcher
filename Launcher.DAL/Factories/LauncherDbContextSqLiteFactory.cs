using Launcher.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace Launcher.DAL.Factories;

public class LauncherDbContextSqLiteFactory : IDbContextFactory<LauncherDbContext>
{
    private readonly DbContextOptionsBuilder<LauncherDbContext> _contextOptionsBuilder = new();
    public LauncherDbContextSqLiteFactory(string databaseName)
    {
        _contextOptionsBuilder.UseSqlite($"Data Source={databaseName};");
    }

    public LauncherDbContext CreateDbContext()
    {
        return new LauncherDbContext(_contextOptionsBuilder.Options);
    }
}