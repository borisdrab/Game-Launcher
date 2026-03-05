using AwesomeAssertions;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Launcher.DAL.Tests;

public class PlatformTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_Platform_Persisted()
    {
        //Arrange
        var entity = new PlatformEntity()
        {
            Name = "PlayStation 4"
        };
        
        //Act
        LauncherDbContextSut.Add(entity);
        await LauncherDbContextSut.SaveChangesAsync();
        
        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var entityFromDb = await dbx.Platforms.FirstAsync(platformEntity => platformEntity.Name == "PlayStation 4");
        entityFromDb.Should().NotBeNull();
        entityFromDb.Should().BeEquivalentTo(entity, options => options
            .Excluding(platformEntity => platformEntity.GameTitlePlatforms)
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
        eldenRingFromDb.GameTitlePlatforms.Should() .Contain(gameTitlePlatformEntity => gameTitlePlatformEntity.PlatformId == pcPlatform.Id);
        eldenRingFromDb.GameTitlePlatforms.Should() .Contain(gameTitlePlatformEntity => gameTitlePlatformEntity.PlatformId == xboxPlatform.Id);
    }
}