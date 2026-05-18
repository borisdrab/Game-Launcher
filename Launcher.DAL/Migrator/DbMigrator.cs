using Launcher.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace Launcher.DAL.Migrator;

public class DbMigrator(IDbContextFactory<LauncherDbContext> dbContextFactory) : IDbMigrator
{
    public void Migrate()
    {
        using var dbContext = dbContextFactory.CreateDbContext();
        dbContext.Database.Migrate();
    }
}