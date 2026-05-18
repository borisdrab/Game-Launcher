using Launcher.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace Launcher.DAL.Seeds;

public class DbSeeder(IDbContextFactory<LauncherDbContext> dbContextFactory) : IDbSeeder
{
    public void Seed()
    {
        using var dbContext = dbContextFactory.CreateDbContext();

        if (dbContext.GameTitles.Any())
        {
            return;
        }

        dbContext.SeedGenres()
            .SeedPlatforms()
            .SeedUsers()
            .SeedGameTitles()
            .SeedReviews()
            .SeedGameTitleGenres()
            .SeedGameTitlePlatforms()
            .SeedLibraries()
            .SeedLibraryTitles();
        
        dbContext.SaveChanges();
    }
}