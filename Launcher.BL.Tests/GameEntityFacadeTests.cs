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
        var results = (await _facade.GetAsync(null, null, null, "CD PROJECT RED", null, false)).ToList();

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
        gameTitle.Reviews.Should().NotBeEmpty();
        gameTitle.AverageRating.Should().NotBeNull();
        gameTitle.AverageRating.Should().Be(gameTitle.Reviews.Average(x => x.Rating));
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
}