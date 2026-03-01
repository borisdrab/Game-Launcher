using Launcher.DAL.Context;
using Launcher.DAL.Factories;

namespace Launcher.DAL.Tests;

public class DbContextTestsBase : IAsyncLifetime
{
    protected readonly LauncherDbContext LauncherDbContextSut;
    protected readonly LauncherDbContextSqLiteFactory DbContextFactory;

    protected DbContextTestsBase()
    {
        DbContextFactory = new LauncherDbContextSqLiteFactory(GetType().FullName!);
        LauncherDbContextSut = DbContextFactory.CreateDbContext();    
    }

    public async Task InitializeAsync() => await LauncherDbContextSut.Database.EnsureCreatedAsync();

    public async Task DisposeAsync()
    {
        await LauncherDbContextSut.Database.EnsureDeletedAsync();
        await LauncherDbContextSut.DisposeAsync();
    }
}