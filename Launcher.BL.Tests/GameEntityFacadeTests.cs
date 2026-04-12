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
        var gameTitles = (await _facade.GetAsync()).ToList();

        gameTitles.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAsync_ById_ReturnsCorrectDetailModel()
    {
        var gameTitle = await _facade.GetAsync(GameTitleSeeds.TheWitcher3.Id);

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
        var gameTitle = await _facade.GetAsync(Guid.NewGuid());

        gameTitle.Should().BeNull();
    }

    [Fact]
    public async Task Save_NewGameTitle_Persisted()
    {
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

        var savedId = await _facade.SaveAsync(newGame);

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

        await _facade.SaveAsync(gameToUpdate);

        var updated = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        updated.Should().NotBeNull();
        updated.Description.Should().Be("Updated description");
        updated.PriceCents.Should().Be(4999);
        updated.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_ExistingGameTitle_Removed()
    {
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

        await _facade.DeleteAsync(savedId);

        var deleted = await _facade.GetAsync(savedId);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_SearchByName_ReturnsOnlyMatching()
    {
        var results = (await _facade.GetAsync("Witcher", null, null, null, null, false)).ToList();

        results.Should().HaveCount(1);
        results[0].Name.Should().Be(GameTitleSeeds.TheWitcher3.Name);
    }

    [Fact]
    public async Task GetAsync_FilterByPegiRating_ReturnsOnlyMatching()
    {
        var results = (await _facade.GetAsync(null, 16, null, null, null, false)).ToList();

        // Upravené pre Elden Ring, pretože Witcher 3 má PEGI 18
        results.Should().HaveCount(1);
        results[0].Id.Should().Be(GameTitleSeeds.EldenRing.Id);
        results[0].PegiRating.Should().Be(16);
    }

    [Fact]
    public async Task GetAsync_FilterByAvailability_ReturnsOnlyMatching()
    {
        var results = (await _facade.GetAsync(null, null, true, null, null, false)).ToList();

        // Obe seedované hry sú "IsAvailable = true"
        results.Should().HaveCount(2);
        results.Should().OnlyContain(x => x.IsAvailable);
    }

    [Fact]
    public async Task GetAsync_FilterByPublisher_ReturnsOnlyMatching()
    {
        // Upravené na "PROJECT" kvôli "CD PROJECT RED" v seedoch
        var results = (await _facade.GetAsync(null, null, null, "CD PROJECT", null, false)).ToList();

        results.Should().HaveCount(1);
        results[0].Id.Should().Be(GameTitleSeeds.TheWitcher3.Id);
    }

    [Fact]
    public async Task GetAsync_SortByName_ReturnsOrdered()
    {
        var results = (await _facade.GetAsync(null, null, null, null, GameTitleSortBy.Name, false)).ToList();

        results.Select(x => x.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetAsync_SortByPriceCents_ReturnsOrdered()
    {
        var results = (await _facade.GetAsync(null, null, null, null, GameTitleSortBy.PriceCents, false)).ToList();

        results.Select(x => x.PriceCents).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetAsync_ById_ReturnsGenresPlatformsAchievementsReviews()
    {
        var gameTitle = await _facade.GetAsync(GameTitleSeeds.TheWitcher3.Id);

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
        var gameTitle = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);

        gameTitle.Should().NotBeNull();
        gameTitle.AchievementCount.Should().Be(gameTitle.Achievements.Count);
        gameTitle.ReviewCount.Should().Be(gameTitle.Reviews.Count);

        if (gameTitle.Reviews.Count > 0)
        {
            gameTitle.AverageRating.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task AddGenreAsync_AddsRelation()
    {
        await _facade.AddGenreAsync(GameTitleSeeds.EldenRing.Id, GenreSeeds.ActionRpg.Id);

        var gameTitle = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        gameTitle.Should().NotBeNull();
        gameTitle.Genres.Should().Contain(x => x.Id == GenreSeeds.ActionRpg.Id);
    }

    [Fact]
    public async Task RemoveGenreAsync_RemovesRelation()
    {
        await _facade.AddGenreAsync(GameTitleSeeds.EldenRing.Id, GenreSeeds.ActionRpg.Id);

        await _facade.RemoveGenreAsync(GameTitleSeeds.EldenRing.Id, GenreSeeds.ActionRpg.Id);

        var gameTitle = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        gameTitle.Should().NotBeNull();
        gameTitle.Genres.Should().NotContain(x => x.Id == GenreSeeds.ActionRpg.Id);
    }

    [Fact]
    public async Task AddPlatformAsync_AddsRelation()
    {
        await _facade.AddPlatformAsync(GameTitleSeeds.TheWitcher3.Id, PlatformSeeds.PlayStation5.Id);

        var gameTitle = await _facade.GetAsync(GameTitleSeeds.TheWitcher3.Id);
        gameTitle.Should().NotBeNull();
        gameTitle.Platforms.Should().Contain(x => x.Id == PlatformSeeds.PlayStation5.Id);
    }

    [Fact]
    public async Task RemovePlatformAsync_RemovesRelation()
    {
        await _facade.AddPlatformAsync(GameTitleSeeds.TheWitcher3.Id, PlatformSeeds.PlayStation5.Id);

        await _facade.RemovePlatformAsync(GameTitleSeeds.TheWitcher3.Id, PlatformSeeds.PlayStation5.Id);

        var gameTitle = await _facade.GetAsync(GameTitleSeeds.TheWitcher3.Id);
        gameTitle.Should().NotBeNull();
        gameTitle.Platforms.Should().NotContain(x => x.Id == PlatformSeeds.PlayStation5.Id);
    }

    /*[Fact]
    public async Task AddAchievementAsync_AddsAchievement()
    {
        var achievement = new AchievementDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Achievement",
            Description = "Achievement Description",
            Points = 25
        };

        await _facade.AddAchievementAsync(GameTitleSeeds.EldenRing.Id, achievement);

        var gameTitle = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        gameTitle.Should().NotBeNull();
        gameTitle.Achievements.Should().Contain(x => x.Name == "Test Achievement");
    }*/

    /*[Fact]
    public async Task AddReviewAsync_AddsReview()
    {
        var review = new ReviewDetailModel
        {
            UserId = UserSeeds.Boris.Id,
            Rating = 4,
            Text = "Very good game",
            CreatedAt = DateTime.UtcNow
        };

        await _facade.AddReviewAsync(GameTitleSeeds.TheWitcher3.Id, review);

        var gameTitle = await _facade.GetAsync(GameTitleSeeds.TheWitcher3.Id);
        gameTitle.Should().NotBeNull();
        gameTitle.Reviews.Should().Contain(x => x.Text == "Very good game");
    }*/

    [Fact]
    public async Task UpdateReviewAsync_UpdatesReview()
    {
        var existing = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        existing.Should().NotBeNull();
        existing.Reviews.Should().NotBeEmpty();

        var review = existing.Reviews.First();
        review.Text = "Updated review text";
        review.Rating = 3;
        review.UpdatedAt = DateTime.UtcNow;

        await _facade.UpdateReviewAsync(GameTitleSeeds.EldenRing.Id, review);

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
        var existing = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        existing.Should().NotBeNull();
        existing.Reviews.Should().NotBeEmpty();

        var reviewId = existing.Reviews.First().Id;

        await _facade.RemoveReviewAsync(reviewId);

        var updated = await _facade.GetAsync(GameTitleSeeds.EldenRing.Id);
        updated.Should().NotBeNull();
        updated.Reviews.Should().NotContain(x => x.Id == reviewId);
    }
}