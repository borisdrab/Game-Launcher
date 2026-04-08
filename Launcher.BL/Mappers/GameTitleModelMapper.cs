using Launcher.BL.Models;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class GameTitleModelMapper
    : ModelMapperBase<GameTitleEntity, GameTitleListModel, GameTitleDetailModel>
{
    public override GameTitleListModel MapToListModel(GameTitleEntity? entity)
        => entity is null
            ? GameTitleListModel.Empty
            : new GameTitleListModel
            {
                Id = entity.Id,
                Name = entity.Name,
                PegiRating = entity.PegiRating,
                PriceCents = entity.PriceCents,
                CoverImageUrl = entity.CoverImageUrl,
                Publisher = entity.Publisher,
                ReleaseDate = entity.ReleaseDate,
                IsAvailable = entity.IsAvailable
            };

    public override GameTitleDetailModel MapToDetailModel(GameTitleEntity? entity)
        => entity is null
            ? GameTitleDetailModel.Empty
            : new GameTitleDetailModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                PegiRating = entity.PegiRating,
                PriceCents = entity.PriceCents,
                CoverImageUrl = entity.CoverImageUrl,
                Publisher = entity.Publisher,
                ReleaseDate = entity.ReleaseDate,
                IsAvailable = entity.IsAvailable,
                
                Genres = entity.GameTitleGenres
                    .Where(x => x.Genre is not null)
                    .Select(x => new GenreModel
                    {
                        Id = x.Genre!.Id,
                        Name = x.Genre.Name
                    })
                    .OrderBy(x => x.Name)
                    .ToList(),

                Platforms = entity.GameTitlePlatforms
                    .Where(x => x.Platform is not null)
                    .Select(x => new PlatformModel
                    {
                        Id = x.Platform!.Id,
                        Name = x.Platform.Name
                    })
                    .OrderBy(x => x.Name)
                    .ToList(),

                Achievements = entity.Achievements
                    .Select(x => new AchievementModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Points = x.Points,
                        UserAchievementCount = x.UserAchievements.Count,
                        CompletedUsersCount = x.UserAchievements.Count(ua => ua.ProgressPercentage == 100),
                        AverageProgressPercentage = x.UserAchievements.Count == 0
                            ? 0
                            : x.UserAchievements.Average(ua => ua.ProgressPercentage)
                    })
                    .OrderBy(x => x.Name)
                    .ToList(),

                Reviews = entity.Reviews
                    .Select(x => new ReviewModel
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Rating = x.Rating,
                        Text = x.Text,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList(),

                AchievementCount = entity.Achievements.Count,
                ReviewCount = entity.Reviews.Count,
                AverageRating = entity.Reviews.Count == 0
                    ? null
                    : entity.Reviews.Average(x => x.Rating)
            };

    public override GameTitleEntity MapToEntity(GameTitleDetailModel model)
        => new()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            PegiRating = model.PegiRating,
            PriceCents = model.PriceCents,
            CoverImageUrl = model.CoverImageUrl,
            Publisher = model.Publisher,
            ReleaseDate = model.ReleaseDate,
            IsAvailable = model.IsAvailable
        };
    
    public GameTitleGenreEntity MapGenreToEntity(Guid gameTitleId, Guid genreId)
        => new()
        {
            GameTitleId = gameTitleId,
            GenreId = genreId,
            GameTitle = null!,
            Genre = null!
        };

    public GameTitlePlatformEntity MapPlatformToEntity(Guid gameTitleId, Guid platformId)
        => new()
        {
            GameTitleId = gameTitleId,
            PlatformId = platformId,
            GameTitle = null!,
            Platform = null!
        };

    public AchievementEntity MapAchievementToEntity(AchievementModel model, Guid gameTitleId)
        => new()
        {
            Id = model.Id,
            GameTitleId = gameTitleId,
            Name = model.Name,
            Description = model.Description,
            Points = model.Points,
            GameTitle = null!
        };

    public ReviewEntity MapReviewToEntity(ReviewModel model, Guid gameTitleId)
        => new()
        {
            Id = model.Id,
            GameTitleId = gameTitleId,
            UserId = model.UserId,
            Rating = model.Rating,
            Text = model.Text,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
            GameTitle = null!,
            User = null!
        };
}