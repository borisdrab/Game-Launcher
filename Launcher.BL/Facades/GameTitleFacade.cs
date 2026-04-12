using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Mappers;
using Launcher.BL.Models;
using Launcher.BL.Repositories;
using Launcher.BL.Repositories.Interfaces;
using Launcher.DAL.Entities;
using Launcher.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace Launcher.BL.Facades;

public class GameTitleFacade(
    LauncherDbContext ctx,
    IGameTitleRepository gameTitleRepository,
    GameTitleModelMapper mapper)
    : FacadeBase<GameTitleEntity, GameTitleListModel, GameTitleDetailModel>(mapper), IGameTitleFacade
{
    public override async Task<IEnumerable<GameTitleListModel>> GetAsync()
        => mapper.MapToListModel(await gameTitleRepository.Get().OrderBy(g => g.Name).ToListAsync());

    public async Task<IEnumerable<GameTitleListModel>> GetAsync(
        string? searchTerm,
        int? pegiRating,
        bool? isAvailable,
        string? publisher,
        GameTitleSortBy? sortBy,
        bool descending)
    {
        return mapper.MapToListModel(
            await gameTitleRepository
                .GetQuery(searchTerm, pegiRating, isAvailable, publisher, sortBy, descending)
                .ToListAsync());
    }

    public override async Task<IEnumerable<GameTitleListModel>> GetAsync(QueryObject query)
    {
        return await Task.FromResult(Enumerable.Empty<GameTitleListModel>());
    }
    
    public override async Task<GameTitleDetailModel?> GetAsync(Guid id)
    {
        GameTitleEntity? entity = await gameTitleRepository
            .Get()
            .Include(x => x.GameTitleGenres)
                .ThenInclude(x => x.Genre)
            .Include(x => x.GameTitlePlatforms)
                .ThenInclude(x => x.Platform)
            .Include(x => x.Reviews)
            .Include(x => x.Achievements)
                .ThenInclude(x => x.UserAchievements)
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity is null ? null : mapper.MapToDetailModel(entity);
    }
    
    public override async Task<Guid> SaveAsync(GameTitleDetailModel model)
    {
        GameTitleEntity entity = mapper.MapToEntity(model);

        if (model.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
            gameTitleRepository.Insert(entity);
            await ctx.SaveChangesAsync();
            return entity.Id;
        }

        await gameTitleRepository.UpdateAsync(entity);
        await ctx.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task DeleteAsync(Guid id)
    {
        await gameTitleRepository.DeleteAsync(id);
        await ctx.SaveChangesAsync();
    }
    
    public async Task AddGenreAsync(Guid gameTitleId, Guid genreId)
    {
        GameTitleEntity? entity = await gameTitleRepository.GetForUpdateAsync(gameTitleId);

        if (entity is null)
        {
            throw new InvalidOperationException($"GameTitle with id {gameTitleId} was not found.");
        }

        if (entity.GameTitleGenres.Any(x => x.GenreId == genreId))
        {
            return;
        }

        entity.GameTitleGenres.Add(mapper.MapGenreToEntity(gameTitleId, genreId));
        await ctx.SaveChangesAsync();
    }

    public async Task RemoveGenreAsync(Guid gameTitleId, Guid genreId)
    {
        GameTitleEntity? entity = await gameTitleRepository.GetForUpdateAsync(gameTitleId);

        if (entity is null)
        {
            throw new InvalidOperationException($"GameTitle with id {gameTitleId} was not found.");
        }

        GameTitleGenreEntity? relation = entity.GameTitleGenres
            .SingleOrDefault(x => x.GenreId == genreId);

        if (relation is null)
        {
            return;
        }

        entity.GameTitleGenres.Remove(relation);
        await ctx.SaveChangesAsync();
    }

    public async Task AddPlatformAsync(Guid gameTitleId, Guid platformId)
    {
        GameTitleEntity? entity = await gameTitleRepository.GetForUpdateAsync(gameTitleId);

        if (entity is null)
        {
            throw new InvalidOperationException($"GameTitle with id {gameTitleId} was not found.");
        }

        if (entity.GameTitlePlatforms.Any(x => x.PlatformId == platformId))
        {
            return;
        }

        entity.GameTitlePlatforms.Add(mapper.MapPlatformToEntity(gameTitleId, platformId));
        await ctx.SaveChangesAsync();
    }

    public async Task RemovePlatformAsync(Guid gameTitleId, Guid platformId)
    {
        GameTitleEntity? entity = await gameTitleRepository.GetForUpdateAsync(gameTitleId);

        if (entity is null)
        {
            throw new InvalidOperationException($"GameTitle with id {gameTitleId} was not found.");
        }

        GameTitlePlatformEntity? relation = entity.GameTitlePlatforms
            .SingleOrDefault(x => x.PlatformId == platformId);

        if (relation is null)
        {
            return;
        }

        entity.GameTitlePlatforms.Remove(relation);
        await ctx.SaveChangesAsync();
    }

    public async Task AddAchievementAsync(Guid gameTitleId, AchievementDetailModel model)
    {
        bool exists = await ctx.GameTitles.AnyAsync(x => x.Id == gameTitleId);

        if (!exists)
        {
            throw new InvalidOperationException($"GameTitle with id {gameTitleId} was not found.");
        }

        AchievementEntity achievement = mapper.MapAchievementToEntity(model, gameTitleId);

        if (achievement.Id == Guid.Empty)
        {
            achievement.Id = Guid.NewGuid();
        }

        ctx.Achievements.Add(achievement);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateAchievementAsync(Guid gameTitleId, AchievementDetailModel model)
    {
        AchievementEntity? existingAchievement = await gameTitleRepository.GetAchievementByIdAsync(model.Id);

        if (existingAchievement is null)
        {
            throw new InvalidOperationException($"Achievement with id {model.Id} was not found.");
        }

        if (existingAchievement.GameTitleId != gameTitleId)
        {
            throw new InvalidOperationException("Achievement does not belong to the specified GameTitle.");
        }

        AchievementEntity mappedAchievement = mapper.MapAchievementToEntity(model, gameTitleId);

        existingAchievement.Name = mappedAchievement.Name;
        existingAchievement.Description = mappedAchievement.Description;
        existingAchievement.Points = mappedAchievement.Points;

        await ctx.SaveChangesAsync();
    }

    public async Task RemoveAchievementAsync(Guid achievementId)
    {
        await gameTitleRepository.DeleteAchievementAsync(achievementId);
        await ctx.SaveChangesAsync();
    }

    public async Task AddReviewAsync(Guid gameTitleId, ReviewDetailModel model)
    {
        bool exists = await ctx.GameTitles.AnyAsync(x => x.Id == gameTitleId);

        if (!exists)
        {
            throw new InvalidOperationException($"GameTitle with id {gameTitleId} was not found.");
        }

        ReviewEntity review = mapper.MapReviewToEntity(model, gameTitleId);

        if (review.Id == Guid.Empty)
        {
            review.Id = Guid.NewGuid();
        }

        ctx.Reviews.Add(review);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateReviewAsync(Guid gameTitleId, ReviewDetailModel model)
    {
        ReviewEntity? existingReview = await gameTitleRepository.GetReviewByIdAsync(model.Id);

        if (existingReview is null)
        {
            throw new InvalidOperationException($"Review with id {model.Id} was not found.");
        }

        if (existingReview.GameTitleId != gameTitleId)
        {
            throw new InvalidOperationException("Review does not belong to the specified GameTitle.");
        }

        ReviewEntity mappedReview = mapper.MapReviewToEntity(model, gameTitleId);

        existingReview.UserId = mappedReview.UserId;
        existingReview.Rating = mappedReview.Rating;
        existingReview.Text = mappedReview.Text;
        existingReview.CreatedAt = mappedReview.CreatedAt;
        existingReview.UpdatedAt = mappedReview.UpdatedAt ?? DateTime.UtcNow;

        await ctx.SaveChangesAsync();
    }

    public async Task RemoveReviewAsync(Guid reviewId)
    {
        await gameTitleRepository.DeleteReviewAsync(reviewId);
        await ctx.SaveChangesAsync();
    }
}