using Launcher.Common.Tests;
using Launcher.DAL.Context;
using Launcher.DAL.Factories;
using Launcher.DAL.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Launcher.DAL.Tests;

public class DbContextTestsBase : IAsyncLifetime
{
    protected DbContextTestsBase(ITestOutputHelper output)
    {
        XUnitTestOutputConverter converter = new(output);
        Console.SetOut(converter);
        
        DbContextFactory = new LauncherDbContextSqLiteFactory(GetType().FullName!);
        LauncherDbContextSut = DbContextFactory.CreateDbContext();    
    }
    
    protected readonly LauncherDbContext LauncherDbContextSut;
    protected readonly IDbContextFactory<LauncherDbContext> DbContextFactory;
    
    public async Task InitializeAsync()
    {
        await LauncherDbContextSut.Database.EnsureDeletedAsync();
        await LauncherDbContextSut.Database.EnsureCreatedAsync();

        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        dbx
            .SeedGameTitles()
            .SeedGenres();
        await dbx.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await LauncherDbContextSut.Database.EnsureDeletedAsync();
        await LauncherDbContextSut.DisposeAsync();
    }
}