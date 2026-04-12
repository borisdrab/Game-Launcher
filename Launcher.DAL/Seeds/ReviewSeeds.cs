using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using static Launcher.DAL.Seeds.UserSeeds;
using static Launcher.DAL.Seeds.GameTitleSeeds;

namespace Launcher.DAL.Seeds;

public static class ReviewSeeds
{
    public static readonly ReviewEntity StepansEldenRingReview = new()
    {
        Id = Guid.Parse("7484433A-6CDA-4C28-AB91-830D40F2D621"),
        UserId = Stepan.Id,
        GameTitleId = EldenRing.Id,
        Rating = 5,
        CreatedAt = DateTime.Now
    };

    public static DbContext SeedReviews(this DbContext dbx)
    {
        dbx.Set<ReviewEntity>().AddRange(
            new ReviewEntity
            {
                Id = StepansEldenRingReview.Id,
                UserId = StepansEldenRingReview.UserId,
                GameTitleId = StepansEldenRingReview.GameTitleId,
                Rating = StepansEldenRingReview.Rating,
                CreatedAt = StepansEldenRingReview.CreatedAt
            }
        );

        return dbx;
    }
}