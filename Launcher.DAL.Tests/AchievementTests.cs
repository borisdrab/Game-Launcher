using AwesomeAssertions;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Launcher.DAL.Tests;

public class AchievementTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_Achievement_Persisted()
    {
        //Arrange
        var eldenRingGame = await LauncherDbContextSut.GameTitles.FirstAsync(game => game.Name == "ELDEN RING");
        var entity = new AchievementEntity()
        {
            Name = "Elden Lord",
            Description = "Achieved the \"Elden Lord\" ending",
            Points = 50,
            GameTitleId = eldenRingGame.Id,
        };
        
        //Act
        LauncherDbContextSut.Add(entity);
        await LauncherDbContextSut.SaveChangesAsync();
        
        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var entityFromDb = await dbx.Achievements
            .Include(achievementEntity => achievementEntity.GameTitle)
            .FirstAsync(achievementEntity => achievementEntity.Id == entity.Id);
        entityFromDb.Should().BeEquivalentTo(entity, options => options
            .Excluding(achievement => achievement.UserAchievements)
            .Excluding(achievement => achievement.GameTitle)
        );
        entityFromDb.GameTitleId.Should().Be(eldenRingGame.Id);
    }

    [Fact]
    public async Task Remove_Achievement_Persisted()
    {
        //Arrange
        var seededGame = await LauncherDbContextSut.GameTitles.FirstAsync();
        var entity = new AchievementEntity()
        {
            Name = "Achievement to be deleted",
            Points = 10,
            GameTitleId = seededGame.Id
        };
        
        LauncherDbContextSut.Add(entity);
        await LauncherDbContextSut.SaveChangesAsync();
        
        //Pre-Assert to check if the achievement has been correctly added to the database
        var existsBeforeDeletion = await LauncherDbContextSut.Achievements.AnyAsync(achievementEntity => achievementEntity.Id == entity.Id);
        Output.WriteLine("Checking if achievement was added to the database successfully before attempting to remove it...");
        existsBeforeDeletion.Should().BeTrue();
        Output.WriteLine("Check succeeded, continuing...");
        
        LauncherDbContextSut.ChangeTracker.Clear();
        
        //Act
        var entityToDelete = await LauncherDbContextSut.Achievements.FindAsync(entity.Id);
        LauncherDbContextSut.Achievements.Remove(entityToDelete!);
        await LauncherDbContextSut.SaveChangesAsync();
        
        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var exists = await dbx.Achievements.AnyAsync(achievementEntity => achievementEntity.Id == entity.Id);
        exists.Should().BeFalse();
    }
}