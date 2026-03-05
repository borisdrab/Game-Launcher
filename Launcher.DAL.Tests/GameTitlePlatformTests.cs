using AwesomeAssertions;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Launcher.DAL.Tests;

public class GameTitlePlatformTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_GameTitlePlatform_Persisted()
    {
        //Arrange
        var seededGame = await LauncherDbContextSut.GameTitles.FirstAsync(gameTitleEntity => gameTitleEntity.Name == "ELDEN RING");
        var seededPlatform = await LauncherDbContextSut.Platforms.FirstAsync(platformEntity => platformEntity.Name == "PC");
        var entity = new GameTitlePlatformEntity()
        {
            GameTitleId = seededGame.Id,
            PlatformId = seededPlatform.Id
        };
        
        //Act
        LauncherDbContextSut.Add(entity);
        await LauncherDbContextSut.SaveChangesAsync();
        
        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var entityFromDb = await dbx.GameTitlePlatforms.FirstAsync(gamePlatformEntity => gamePlatformEntity.GameTitleId == entity.GameTitleId &&  gamePlatformEntity.PlatformId == entity.PlatformId);
        entityFromDb.Should().NotBeNull();
        entityFromDb.Should().BeEquivalentTo(entity, options => options
            .Excluding(gamePlatformEntity => gamePlatformEntity.GameTitle)
            .Excluding(gamePlatformEntity => gamePlatformEntity.Platform)
        );
    }
}