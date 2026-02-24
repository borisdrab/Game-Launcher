using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Launcher.DAL.Context;

public class LauncherDbContextFactory : IDesignTimeDbContextFactory<LauncherDbContext>
{
    public LauncherDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LauncherDbContext>();
        optionsBuilder.UseSqlite("Data Source=launcher.db");
        
        return new LauncherDbContext(optionsBuilder.Options);
    }
}
