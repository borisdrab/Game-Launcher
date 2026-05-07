using AwesomeAssertions;
using Launcher.BL.Facades;
using Launcher.BL.Mappers;
using Launcher.BL.Models;
using Launcher.DAL.Seeds;
using Xunit.Abstractions;

namespace Launcher.BL.Tests;

public class ReviewFacadeTests : FacadeTestsBase
{
    private readonly ReviewFacade _facade;

    public ReviewFacadeTests(ITestOutputHelper output) : base(output)
    {
        var mapper = new ReviewModelMapper();
        _facade = new ReviewFacade(mapper, DbContextFactory);
    }

    [Fact]
    public async Task GetAll_ReturnsSeededReviews()
    {
        // Act
        var reviews = await _facade.GetAsync();

        // Assert - there are 3 seeded reviews
        var reviewList = reviews.ToList();
        reviewList.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetById_ReturnsCorrectReview()
    {
        // Act - get the seeded review
        var review = await _facade.GetAsync(ReviewSeeds.StepansEldenRingReview.Id);

        // Assert
        review.Should().NotBeNull();
        review.Rating.Should().Be(5);
        review.UserId.Should().Be(UserSeeds.Stepan.Id);
        review.GameTitleId.Should().Be(GameTitleSeeds.EldenRing.Id);
    }

    [Fact]
    public async Task Save_NewReview_Persisted()
    {
        // Arrange - Stepan writes a review for The Witcher 3
        var newReview = new ReviewDetailModel();
        newReview.UserId = UserSeeds.Stepan.Id;
        newReview.GameTitleId = GameTitleSeeds.TheWitcher3.Id;
        newReview.Rating = 4;
        newReview.Text = "Great game!";

        // Act
        var savedId = await _facade.SaveAsync(newReview);

        // Assert
        var reviewFromDb = await _facade.GetAsync(savedId);
        reviewFromDb.Should().NotBeNull();
        reviewFromDb.Rating.Should().Be(4);
        reviewFromDb.Text.Should().Be("Great game!");
    }

    [Fact]
    public async Task Save_UpdateExistingReview_RatingChanged()
    {
        // Arrange - update Stepan's review rating from 5 to 3
        var reviewToUpdate = new ReviewDetailModel();
        reviewToUpdate.Id = ReviewSeeds.StepansEldenRingReview.Id;
        reviewToUpdate.UserId = UserSeeds.Stepan.Id;
        reviewToUpdate.GameTitleId = GameTitleSeeds.EldenRing.Id;
        reviewToUpdate.Rating = 3;
        reviewToUpdate.Text = "Changed my mind";

        // Act
        await _facade.SaveAsync(reviewToUpdate);

        // Assert
        var updatedReview = await _facade.GetAsync(ReviewSeeds.StepansEldenRingReview.Id);
        updatedReview.Should().NotBeNull();
        updatedReview.Rating.Should().Be(3);
        updatedReview.Text.Should().Be("Changed my mind");
        updatedReview.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_ExistingReview_Removed()
    {
        // Act
        await _facade.DeleteAsync(ReviewSeeds.StepansEldenRingReview.Id);

        // Assert
        var deletedReview = await _facade.GetAsync(ReviewSeeds.StepansEldenRingReview.Id);
        deletedReview.Should().BeNull();
    }

    [Fact]
    public async Task Query_SortByRatingDescending_HighestFirst()
    {
        // Arrange - use seeded reviews (ratings: 5, 4, 2)
        var query = new QueryObject();
        query.SortBy = "Rating";
        query.SortDescending = true;

        // Act
        var results = await _facade.GetAsync(query);

        // Assert - ratings should be sorted: 5, 4, 2
        var resultList = results.ToList();
        resultList.Should().HaveCount(3);
        resultList[0].Rating.Should().Be(5);
        resultList[1].Rating.Should().Be(4);
        resultList[2].Rating.Should().Be(2);
    }

    [Fact]
    public async Task Query_SortByRatingAscending_LowestFirst()
    {
        // Arrange - use seeded reviews (ratings: 5, 4, 2)
        var query = new QueryObject();
        query.SortBy = "Rating";
        query.SortDescending = false;

        // Act
        var results = await _facade.GetAsync(query);

        // Assert - ratings should be sorted: 2, 4, 5
        var resultList = results.ToList();
        resultList.Should().HaveCount(3);
        resultList[0].Rating.Should().Be(2);
        resultList[1].Rating.Should().Be(4);
        resultList[2].Rating.Should().Be(5);
    }

    [Fact]
    public async Task Query_SortByDateDescending_NewestFirst()
    {
        // Arrange - seeded dates: 2026-01-10, 2026-02-15, 2026-03-20
        var query = new QueryObject();
        query.SortBy = "CreatedAt";
        query.SortDescending = true;

        // Act
        var results = await _facade.GetAsync(query);

        // Assert - newest review (March) should be first
        var resultList = results.ToList();
        resultList.Should().HaveCount(3);
        resultList[0].CreatedAt.Should().Be(new DateTime(2026, 3, 20));
        resultList[2].CreatedAt.Should().Be(new DateTime(2026, 1, 10));
    }
}
