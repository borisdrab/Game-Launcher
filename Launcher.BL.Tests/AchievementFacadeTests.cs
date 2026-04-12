using AwesomeAssertions;
using Launcher.BL.Facades;
using Launcher.BL.Mappers;
using Launcher.BL.Models;
using Launcher.DAL.Seeds;
using Xunit.Abstractions;

namespace Launcher.BL.Tests;

public class AchievementFacadeTests : FacadeTestsBase
{
    private readonly AchievementFacade _facade;

    public AchievementFacadeTests(ITestOutputHelper output) : base(output)
    {
        var mapper = new AchievementModelMapper();
        _facade = new AchievementFacade(mapper, DbContextFactory);
    }

    [Fact]
    public async Task Save_NewAchievement_Persisted()
    {
        // Arrange - create an achievement for Elden Ring
        var newAchievement = new AchievementDetailModel();
        newAchievement.Name = "Elden Lord";
        newAchievement.Description = "Achieve the Elden Lord ending";
        newAchievement.Points = 100;
        newAchievement.GameTitleId = GameTitleSeeds.EldenRing.Id;

        // Act
        var savedId = await _facade.SaveAsync(newAchievement);

        // Assert
        var achievementFromDb = await _facade.GetAsync(savedId);
        achievementFromDb.Should().NotBeNull();
        achievementFromDb!.Name.Should().Be("Elden Lord");
        achievementFromDb.Points.Should().Be(100);
        achievementFromDb.GameTitleId.Should().Be(GameTitleSeeds.EldenRing.Id);
    }

    [Fact]
    public async Task Save_UpdateExistingAchievement_NameChanged()
    {
        // Arrange - create an achievement first
        var achievement = new AchievementDetailModel();
        achievement.Name = "Old Name";
        achievement.Points = 50;
        achievement.GameTitleId = GameTitleSeeds.EldenRing.Id;
        var savedId = await _facade.SaveAsync(achievement);

        // Now update it
        var achievementToUpdate = new AchievementDetailModel();
        achievementToUpdate.Id = savedId;
        achievementToUpdate.Name = "New Name";
        achievementToUpdate.Points = 75;
        achievementToUpdate.GameTitleId = GameTitleSeeds.EldenRing.Id;

        // Act
        await _facade.SaveAsync(achievementToUpdate);

        // Assert
        var updated = await _facade.GetAsync(savedId);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("New Name");
        updated.Points.Should().Be(75);
    }

    [Fact]
    public async Task Delete_ExistingAchievement_Removed()
    {
        // Arrange - create an achievement first
        var achievement = new AchievementDetailModel();
        achievement.Name = "To Delete";
        achievement.Points = 10;
        achievement.GameTitleId = GameTitleSeeds.EldenRing.Id;
        var savedId = await _facade.SaveAsync(achievement);

        // Act
        await _facade.DeleteAsync(savedId);

        // Assert
        var deleted = await _facade.GetAsync(savedId);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Query_SearchByName_ReturnsMatching()
    {
        // Arrange - create two achievements
        var ach1 = new AchievementDetailModel();
        ach1.Name = "Dragon Slayer";
        ach1.Points = 50;
        ach1.GameTitleId = GameTitleSeeds.EldenRing.Id;
        await _facade.SaveAsync(ach1);

        var ach2 = new AchievementDetailModel();
        ach2.Name = "First Steps";
        ach2.Points = 10;
        ach2.GameTitleId = GameTitleSeeds.EldenRing.Id;
        await _facade.SaveAsync(ach2);

        var query = new QueryObject();
        query.SearchTerm = "Dragon";

        // Act
        var results = await _facade.GetAsync(query);

        // Assert
        var resultList = results.ToList();
        resultList.Should().HaveCount(1);
        resultList[0].Name.Should().Be("Dragon Slayer");
    }

    [Fact]
    public async Task Query_SortByPointsDescending_HighestFirst()
    {
        // Arrange - create two achievements with different points
        var ach1 = new AchievementDetailModel();
        ach1.Name = "Easy";
        ach1.Points = 10;
        ach1.GameTitleId = GameTitleSeeds.EldenRing.Id;
        await _facade.SaveAsync(ach1);

        var ach2 = new AchievementDetailModel();
        ach2.Name = "Hard";
        ach2.Points = 100;
        ach2.GameTitleId = GameTitleSeeds.EldenRing.Id;
        await _facade.SaveAsync(ach2);

        var query = new QueryObject();
        query.SortBy = "Points";
        query.SortDescending = true;

        // Act
        var results = await _facade.GetAsync(query);

        // Assert - 100 points should come first
        var resultList = results.ToList();
        resultList[0].Points.Should().Be(100);
    }
}
