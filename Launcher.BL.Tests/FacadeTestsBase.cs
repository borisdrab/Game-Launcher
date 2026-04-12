using Launcher.Common.Tests;
using Launcher.DAL.Context;
using Launcher.DAL.Factories;
using Launcher.DAL.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Launcher.BL.Tests;

public class FacadeTestsBase : IAsyncLifetime
{
    protected readonly IDbContextFactory<LauncherDbContext> DbContextFactory;

    protected FacadeTestsBase(ITestOutputHelper output)
    {
        // Redirect console output to xUnit
        XUnitTestOutputConverter converter = new(output);
        Console.SetOut(converter);

        // Create a factory that uses a unique database for each test class
        DbContextFactory = new LauncherDbContextSqLiteFactory(GetType().FullName!);
    }

    public async Task InitializeAsync()
    {
        // Create a fresh database and seed it with test data
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        dbContext
            .SeedGameTitles()
            .SeedGenres()
            .SeedGameTitleGenres()
            .SeedUsers()
            .SeedLibraries()
            .SeedLibraryTitles()
            .SeedPlatforms()
            .SeedGameTitlePlatforms()
            .SeedReviews();

        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        // Clean up the database after tests
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();
        await dbContext.Database.EnsureDeletedAsync();
    }
}
