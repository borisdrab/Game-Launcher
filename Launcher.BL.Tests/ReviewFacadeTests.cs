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

        // Assert - there is 1 seeded review (Elden Ring review)
        var reviewList = reviews.ToList();
        reviewList.Should().HaveCount(1);
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
        // Arrange - Boris writes a review for The Witcher 3
        var newReview = new ReviewDetailModel();
        newReview.UserId = UserSeeds.Boris.Id;
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
        // Arrange - add a second review so we can test sorting
        var secondReview = new ReviewDetailModel();
        secondReview.UserId = UserSeeds.Boris.Id;
        secondReview.GameTitleId = GameTitleSeeds.TheWitcher3.Id;
        secondReview.Rating = 2;
        await _facade.SaveAsync(secondReview);

        var query = new QueryObject();
        query.SortBy = "Rating";
        query.SortDescending = true;

        // Act
        var results = await _facade.GetAsync(query);

        // Assert - rating 5 should come before rating 2
        var resultList = results.ToList();
        resultList.Should().HaveCount(2);
        resultList[0].Rating.Should().Be(5);
        resultList[1].Rating.Should().Be(2);
    }
}
