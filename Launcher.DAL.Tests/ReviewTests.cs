using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Launcher.DAL.Tests;

public class ReviewTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task SeededReview_ExistsIn_UserAndGameTitleCollections()
    {
        //Arrange
        var reviewUser = await LauncherDbContextSut.Users.FirstAsync(userEntity => userEntity.DisplayName == "stepkren");
        var reviewGame = await LauncherDbContextSut.GameTitles.FirstAsync(gameEntity => gameEntity.Name == "ELDEN RING");
        var seededReviewEntity = await LauncherDbContextSut.Reviews.FirstAsync(reviewEntity => reviewEntity.UserId == reviewUser.Id && reviewEntity.GameTitleId == reviewGame.Id);
        
        //Act
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var userFromDb = await dbx.Users
            .Include(userEntity => userEntity.Reviews)
            .FirstAsync(userEntity => userEntity.DisplayName == "stepkren");
        var gameFromDb = await dbx.GameTitles
            .Include(gameEntity => gameEntity.Reviews)
            .FirstAsync(gameEntity => gameEntity.Name == "ELDEN RING");
        
        //Assert
        userFromDb.Should().NotBeNull();
        userFromDb.Reviews.Should().Contain(reviewEntity => reviewEntity.Id == seededReviewEntity.Id).And.HaveCountGreaterThanOrEqualTo(1);
        
        gameFromDb.Should().NotBeNull();
        gameFromDb.Reviews.Should().Contain(reviewEntity => reviewEntity.Id == seededReviewEntity.Id).And.HaveCountGreaterThanOrEqualTo(1);
        
        var gameReview = gameFromDb.Reviews.First(reviewEntity => reviewEntity.Id ==  seededReviewEntity.Id);
        gameReview.Rating.Should().Be(5);
        gameReview.UserId.Should().Be(reviewUser.Id);
    }
}