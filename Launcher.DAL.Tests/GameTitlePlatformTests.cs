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
        var seededPlatform = await LauncherDbContextSut.Platforms.FirstAsync(platformEntity => platformEntity.Name == "PlayStation 5");
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
    
    [Fact]
    public async Task EldenRing_IsLinkedTo_BothPcAndXbox()
    {
        //Arrange
        var pcPlatform = await LauncherDbContextSut.Platforms.FirstAsync(platform => platform.Name == "PC");
        var xboxPlatform = await LauncherDbContextSut.Platforms.FirstAsync(platform => platform.Name == "Xbox");
        
        //Act
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var eldenRingFromDb = await dbx.GameTitles
            .Include(gameTitleEntity => gameTitleEntity.GameTitlePlatforms)
            .FirstAsync(gameTitleEntity => gameTitleEntity.Name == "ELDEN RING");
        
        //Assert
        eldenRingFromDb.Should().NotBeNull();
        eldenRingFromDb.GameTitlePlatforms.Should().HaveCount(2);
        eldenRingFromDb.GameTitlePlatforms.Should().Contain(gameTitlePlatformEntity => gameTitlePlatformEntity.PlatformId == pcPlatform.Id);
        eldenRingFromDb.GameTitlePlatforms.Should().Contain(gameTitlePlatformEntity => gameTitlePlatformEntity.PlatformId == xboxPlatform.Id);
    }
}