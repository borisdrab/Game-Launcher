using AwesomeAssertions;
using Launcher.BL.Facades;
using Launcher.BL.Mappers;
using Launcher.BL.Models;
using Launcher.BL.Repositories;
using Launcher.DAL.Seeds;
using Xunit.Abstractions;

namespace Launcher.BL.Tests;

public class GameTitleFacadeTests : FacadeTestsBase
{
    private readonly GameTitleFacade _facade;

    public GameTitleFacadeTests(ITestOutputHelper output) : base(output)
    {
        var ctx = DbContextFactory.CreateDbContext();
        var repository = new GameTitleRepository(ctx, new GameTitleEntityMapper());
        _facade = new GameTitleFacade(ctx, repository, new GameTitleModelMapper());
    }

    [Fact]
    public async Task GetAsync_ReturnsAllSeededGameTitles()
    {
        // Act
        var gameTitles = (await _facade.GetAsync()).ToList();

        //  Assert
        gameTitles.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAsync_ById_ReturnsCorrectDetailModel()
    {
        // Act
        var gameTitle = await _facade.GetAsync(GameTitleSeeds.TheWitcher3.Id);

        // Assert
        gameTitle.Should().NotBeNull();
        gameTitle.Id.Should().Be(GameTitleSeeds.TheWitcher3.Id);
        gameTitle.Name.Should().Be(GameTitleSeeds.TheWitcher3.Name);
        gameTitle.Description.Should().Be(GameTitleSeeds.TheWitcher3.Description);
        gameTitle.PegiRating.Should().Be(GameTitleSeeds.TheWitcher3.PegiRating);
        gameTitle.PriceCents.Should().Be(GameTitleSeeds.TheWitcher3.PriceCents);
        gameTitle.Publisher.Should().Be(GameTitleSeeds.TheWitcher3.Publisher);
        gameTitle.IsAvailable.Should().Be(GameTitleSeeds.TheWitcher3.IsAvailable);
    }

    [Fact]
    public async Task GetAsync_ById_NonExistingId_ReturnsNull()
    {
        // Act
        var gameTitle = await _facade.GetAsync(Guid.NewGuid());
        
        // Assert
        gameTitle.Should().BeNull();
    }

    [Fact]
    public async Task Save_NewGameTitle_Persisted()
    {
        // Arrange
        var newGame = new GameTitleDetailModel
        {
            Name = "Test Game",
            Description = "Test Description",
            PegiRating = 18,
            PriceCents = 1999,
            CoverImageUrl = "test-cover.png",
            Publisher = "Test Publisher",
            ReleaseDate = new DateTime(2024, 1, 1),
            IsAvailable = true
        };

        // Act
        var savedId = await _facade.SaveAsync(newGame);

        // Assert
        var gameFromDb = await _facade.GetAsync(savedId);
        gameFromDb.Should().NotBeNull();
        gameFromDb.Name.Should().Be("Test Game");
        gameFromDb.Description.Should().Be("Test Description");
        gameFromDb.PriceCents.Should().Be(1999);
        gameFromDb.PegiRating.Should().Be(18);
    }

    [Fact]
    public async Task Save_UpdateExistingGameTitle_ChangesPersisted()
    {
        // Arrange
        var gameToUpdate = new GameTitleDetailModel
        {
            Id = GameTitleSeeds.EldenRing.Id,
            Name = GameTitleSeeds.EldenRing.Name,
            Description = "Updated description",
            PegiRating = GameTitleSeeds.EldenRing.PegiRating,
            PriceCents = 4999,
            CoverImageUrl = GameTitleSeeds.EldenRing.CoverImageUrl,
            Publisher = GameTitleSeeds.EldenRing.Publisher,
            ReleaseDate = GameTitleSeeds.EldenRing.ReleaseDate,
            IsAvailable = false
        };

        // Act
        await _facade.SaveAsync(gameToUpdate);

        // Assert
        var updated = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        updated.Should().NotBeNull();
        updated.Description.Should().Be("Updated description");
        updated.PriceCents.Should().Be(4999);
        updated.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_ExistingGameTitle_Removed()
    {
        // Arrange
        var newGame = new GameTitleDetailModel
        {
            Name = "ToDelete",
            Description = "Delete me",
            PegiRating = 12,
            PriceCents = 1000,
            CoverImageUrl = "",
            Publisher = "Delete Studio",
            ReleaseDate = new DateTime(2024, 1, 1),
            IsAvailable = true
        };

        var savedId = await _facade.SaveAsync(newGame);

        // Act
        await _facade.DeleteAsync(savedId);

        // Assert
        var deleted = await _facade.GetAsync(savedId);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_SearchByName_ReturnsOnlyMatching()
    {
        // Act
        var results = (await _facade.GetAsync("Witcher", null, null, null, null, false)).ToList();

        // Assert
        results.Should().HaveCount(1);
        results[0].Name.Should().Be(GameTitleSeeds.TheWitcher3.Name);
    }

    [Fact]
    public async Task GetAsync_FilterByPegiRating_ReturnsOnlyMatching()
    {
        // Act
        var results = (await _facade.GetAsync(null, 16, null, null, null, false)).ToList();

        // Assert
        results.Should().HaveCount(1);
        results[0].Id.Should().Be(GameTitleSeeds.EldenRing.Id);
        results[0].PegiRating.Should().Be(16);
    }

    [Fact]
    public async Task GetAsync_FilterByAvailability_ReturnsOnlyMatching()
    {
        // Act
        var results = (await _facade.GetAsync(null, null, true, null, null, false)).ToList();

        // Assert
        results.Should().HaveCount(2);
        results.Should().OnlyContain(x => x.IsAvailable);
    }

    [Fact]
    public async Task GetAsync_FilterByPublisher_ReturnsOnlyMatching()
    {
        // Act
        var results = (await _facade.GetAsync(null, null, null, "CD PROJECT", null, false)).ToList();

        // Assert
        results.Should().HaveCount(1);
        results[0].Id.Should().Be(GameTitleSeeds.TheWitcher3.Id);
    }

    [Fact]
    public async Task GetAsync_SortByName_ReturnsOrdered()
    {
        // Act
        var results = (await _facade.GetAsync(null, null, null, null, GameTitleSortBy.Name, false)).ToList();

        // Assert
        results.Select(x => x.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetAsync_SortByPriceCents_ReturnsOrdered()
    {
        // Act
        var results = (await _facade.GetAsync(null, null, null, null, GameTitleSortBy.PriceCents, false)).ToList();

        // Assert
        results.Select(x => x.PriceCents).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetAsync_ById_ReturnsGenresPlatformsAchievementsReviews()
    {
        // Act
        var gameTitle = await _facade.GetAsync(GameTitleSeeds.TheWitcher3.Id);

        // Assert
        gameTitle.Should().NotBeNull();
        gameTitle.Genres.Should().NotBeNull();
        gameTitle.Platforms.Should().NotBeNull();
        gameTitle.Achievements.Should().NotBeNull();
        gameTitle.Reviews.Should().NotBeNull();
        gameTitle.Genres.Should().Contain(x => x.Id == GenreSeeds.OpenWorld.Id);
        gameTitle.Genres.Should().Contain(x => x.Id == GenreSeeds.Rpg.Id);
    }

    [Fact]
    public async Task GetAsync_ById_ReturnsAverageRatingAndCounts()
    {
        // Act
        var gameTitle = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);

        // Assert
        gameTitle.Should().NotBeNull();
        gameTitle.AchievementCount.Should().Be(gameTitle.Achievements.Count);
        gameTitle.ReviewCount.Should().Be(gameTitle.Reviews.Count);

        if (gameTitle.Reviews.Count > 0)
        {
            gameTitle.AverageRating.Should().NotBeNull();
        }
    }
    
    [Fact]
    public async Task GetAsync_QueryObject_SearchAndSort_Works()
    {
        // Arrange
        var query = new QueryObject
        {
            SearchTerm = "Witcher",
            SortBy = "Name",
            SortDescending = false
        };

        // Act
        var results = (await _facade.GetAsync(query)).ToList();

        // Assert
        results.Should().HaveCount(1);
        results[0].Id.Should().Be(GameTitleSeeds.TheWitcher3.Id);
        results[0].Name.Should().Be(GameTitleSeeds.TheWitcher3.Name);
    }

    [Fact]
    public async Task AddGenreAsync_AddsRelation()
    {
        // Act
        await _facade.AddGenreAsync(GameTitleSeeds.EldenRing.Id, GenreSeeds.ActionRpg.Id);

        // Assert
        var gameTitle = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        gameTitle.Should().NotBeNull();
        gameTitle.Genres.Should().Contain(x => x.Id == GenreSeeds.ActionRpg.Id);
    }

    [Fact]
    public async Task RemoveGenreAsync_RemovesRelation()
    {
        // Arrange
        await _facade.AddGenreAsync(GameTitleSeeds.EldenRing.Id, GenreSeeds.ActionRpg.Id);

        // Act
        await _facade.RemoveGenreAsync(GameTitleSeeds.EldenRing.Id, GenreSeeds.ActionRpg.Id);

        // Assert
        var gameTitle = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        gameTitle.Should().NotBeNull();
        gameTitle.Genres.Should().NotContain(x => x.Id == GenreSeeds.ActionRpg.Id);
    }

    [Fact]
    public async Task AddPlatformAsync_AddsRelation()
    {
        // Act
        await _facade.AddPlatformAsync(GameTitleSeeds.TheWitcher3.Id, PlatformSeeds.PlayStation5.Id);

        // Assert
        var gameTitle = await _facade.GetAsync(GameTitleSeeds.TheWitcher3.Id);
        gameTitle.Should().NotBeNull();
        gameTitle.Platforms.Should().Contain(x => x.Id == PlatformSeeds.PlayStation5.Id);
    }

    [Fact]
    public async Task RemovePlatformAsync_RemovesRelation()
    {
        // Arrange
        await _facade.AddPlatformAsync(GameTitleSeeds.TheWitcher3.Id, PlatformSeeds.PlayStation5.Id);

        // Act
        await _facade.RemovePlatformAsync(GameTitleSeeds.TheWitcher3.Id, PlatformSeeds.PlayStation5.Id);

        // Assert
        var gameTitle = await _facade.GetAsync(GameTitleSeeds.TheWitcher3.Id);
        gameTitle.Should().NotBeNull();
        gameTitle.Platforms.Should().NotContain(x => x.Id == PlatformSeeds.PlayStation5.Id);
    }

    [Fact]
    public async Task AddAchievementAsync_AddsAchievement()
    {
        // Arrange
        var achievement = new AchievementDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Achievement",
            Description = "Achievement Description",
            Points = 25
        };

        // Act
        await _facade.AddAchievementAsync(GameTitleSeeds.EldenRing.Id, achievement);

        // Assert
        var gameTitle = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        gameTitle.Should().NotBeNull();
        gameTitle.Achievements.Should().Contain(x => x.Name == "Test Achievement");
    }
    
    [Fact]
    public async Task UpdateAchievementAsync_UpdatesAchievement()
    {
        // Arrange
        var newAchievement = new AchievementDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "Achievement To Remove",
            Description = "Remove me",
            Points = 15
        };
        
        await _facade.AddAchievementAsync(GameTitleSeeds.EldenRing.Id, newAchievement);
        
        var existing = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        existing.Should().NotBeNull();
        existing.Achievements.Should().NotBeEmpty();

        var achievement = existing.Achievements.First();
        achievement.Name = "Updated Achievement Name";
        achievement.Description = "Updated Achievement Description";
        achievement.Points = 99;

        // Act
        await _facade.UpdateAchievementAsync(GameTitleSeeds.EldenRing.Id, achievement);

        // Assert
        var updated = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        updated.Should().NotBeNull();
        updated.Achievements.Should().Contain(x =>
            x.Id == achievement.Id &&
            x.Name == "Updated Achievement Name" &&
            x.Description == "Updated Achievement Description" &&
            x.Points == 99);
    }

    [Fact]
    public async Task RemoveAchievementAsync_RemovesAchievement()
    {
        // Arrange
        var newAchievement = new AchievementDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "Achievement To Remove",
            Description = "Remove me",
            Points = 15
        };

        await _facade.AddAchievementAsync(GameTitleSeeds.EldenRing.Id, newAchievement);
        
        var existing = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        existing.Should().NotBeNull();
        existing.Achievements.Should().NotBeEmpty();

        var achievementId = existing.Achievements.First().Id;

        // Act
        await _facade.RemoveAchievementAsync(achievementId);

        // Assert
        var updated = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        updated.Should().NotBeNull();
        updated.Achievements.Should().NotContain(x => x.Id == achievementId);
    }

    [Fact]
    public async Task AddReviewAsync_AddsReview()
    {
        // Arrange
        var review = new ReviewDetailModel
        {
            UserId = UserSeeds.Boris.Id,
            Rating = 4,
            Text = "Very good game",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _facade.AddReviewAsync(GameTitleSeeds.TheWitcher3.Id, review);

        // Assert
        var gameTitle = await _facade.GetAsync(GameTitleSeeds.TheWitcher3.Id);
        gameTitle.Should().NotBeNull();
        gameTitle.Reviews.Should().Contain(x => x.Text == "Very good game");
    }

    [Fact]
    public async Task UpdateReviewAsync_UpdatesReview()
    {
        // Arrange
        var existing = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        existing.Should().NotBeNull();
        existing.Reviews.Should().NotBeEmpty();

        var review = existing.Reviews.First();
        review.Text = "Updated review text";
        review.Rating = 3;
        review.UpdatedAt = DateTime.UtcNow;

        // Act
        await _facade.UpdateReviewAsync(GameTitleSeeds.EldenRing.Id, review);

        // Assert
        var updated = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        updated.Should().NotBeNull();
        updated.Reviews.Should().Contain(x =>
            x.Id == review.Id &&
            x.Text == "Updated review text" &&
            x.Rating == 3);
    }

    [Fact]
    public async Task RemoveReviewAsync_RemovesReview()
    {
        // Arrange
        var existing = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        existing.Should().NotBeNull();
        existing.Reviews.Should().NotBeEmpty();

        var reviewId = existing.Reviews.First().Id;

        // Act
        await _facade.RemoveReviewAsync(reviewId);

        // Assert
        var updated = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        updated.Should().NotBeNull();
        updated.Reviews.Should().NotContain(x => x.Id == reviewId);
    }
}