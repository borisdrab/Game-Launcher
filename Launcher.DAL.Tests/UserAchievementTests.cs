using AwesomeAssertions;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Launcher.DAL.Tests;

public class UserAchievementTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_UserAchievement_LinkPersisted()
    {
        //Arrange
        var seededUser = await LauncherDbContextSut.Users.FirstAsync(userEntity => userEntity.UserName == "Samuel");
        var seededGame = await LauncherDbContextSut.GameTitles.FirstAsync(gameEntity => gameEntity.Name == "The Witcher 3: Wild Hunt");
        var achievement = new AchievementEntity()
        {
            GameTitleId = seededGame.Id,
            Name = "Ran the Gauntlet",
            Description = "Finish the game on the \"Blood and Broken Bones!\" or \"Death Match!\" difficulty levels.",
            Points = 80
        };
        
        LauncherDbContextSut.Achievements.Add(achievement);
        await LauncherDbContextSut.SaveChangesAsync();

        var userAchievement = new UserAchievementEntity()
        {
            UserId = seededUser.Id,
            AchievementId = achievement.Id,
            UnlockedAt = DateTime.Now,
            ProgressPercentage = 100
        };
        
        //Act
        LauncherDbContextSut.UserAchievements.Add(userAchievement);
        await LauncherDbContextSut.SaveChangesAsync();
        
        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var persistedLink = await dbx.UserAchievements
            .Include(userAchievementEntity => userAchievementEntity.Achievement)
            .Include(userAchievementEntity => userAchievementEntity.User)
            .FirstAsync(userAchievementEntity => userAchievementEntity.UserId == seededUser.Id && userAchievementEntity.AchievementId == achievement.Id);
        persistedLink.Should().NotBeNull();
        persistedLink.AchievementId.Should().Be(achievement.Id);
        persistedLink.UserId.Should().Be(seededUser.Id);
        persistedLink.ProgressPercentage.Should().Be(100);
    }
}