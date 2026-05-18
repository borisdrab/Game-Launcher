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
        Text = "Amazing game, truly a masterpiece!",
        CreatedAt = new DateTime(2026, 1, 10)
    };

    public static readonly ReviewEntity BorisEldenRingReview = new()
    {
        Id = Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890"),
        UserId = Boris.Id,
        GameTitleId = EldenRing.Id,
        Rating = 4,
        Text = "Great story and world",
        CreatedAt = new DateTime(2026, 2, 15)
    };

    public static readonly ReviewEntity SamuelEldenRingReview = new()
    {
        Id = Guid.Parse("B2C3D4E5-F6A7-8901-BCDE-F12345678901"),
        UserId = Samuel.Id,
        GameTitleId = EldenRing.Id,
        Rating = 2,
        Text = "Too difficult for me",
        CreatedAt = new DateTime(2026, 3, 20)
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
                Text = StepansEldenRingReview.Text,
                CreatedAt = StepansEldenRingReview.CreatedAt
            },
            new ReviewEntity
            {
                Id = BorisEldenRingReview.Id,
                UserId = BorisEldenRingReview.UserId,
                GameTitleId = BorisEldenRingReview.GameTitleId,
                Rating = BorisEldenRingReview.Rating,
                Text = BorisEldenRingReview.Text,
                CreatedAt = BorisEldenRingReview.CreatedAt
            },
            new ReviewEntity
            {
                Id = SamuelEldenRingReview.Id,
                UserId = SamuelEldenRingReview.UserId,
                GameTitleId = SamuelEldenRingReview.GameTitleId,
                Rating = SamuelEldenRingReview.Rating,
                Text = SamuelEldenRingReview.Text,
                CreatedAt = SamuelEldenRingReview.CreatedAt
            }
        );

        return dbx;
    }
}