using Launcher.DAL.Entities;
using Launcher.DAL.Context;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Launcher.BL.Repositories;

public class GameTitleRepository : Repository<GameTitleEntity>, IGameTitleRepository
{
    private readonly LauncherDbContext _dbContext;

    public GameTitleRepository(
        LauncherDbContext dbContext,
        IEntityMapper<GameTitleEntity> entityMapper)
        : base(dbContext, entityMapper)
    {
        _dbContext = dbContext;
    }
    
    public IQueryable<GameTitleEntity> GetQuery(
        string? searchTerm,
        int? pegiRating,
        bool? isAvailable,
        string? publisher,
        GameTitleSortBy? sortBy,
        bool descending)
    {
        IQueryable<GameTitleEntity> query = Get();
        query = ApplyFilter(query, searchTerm, pegiRating, isAvailable, publisher);
        query = ApplySort(query, sortBy, descending);
        return query;
    }

    public async Task<GameTitleEntity?> GetForUpdateAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.GameTitles
            .Include(x => x.GameTitleGenres)
            .Include(x => x.GameTitlePlatforms)
            .Include(x => x.Achievements)
            .Include(x => x.Reviews)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    
    public async Task<AchievementEntity?> GetAchievementByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Achievements
            .Include(x => x.UserAchievements)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ReviewEntity?> GetReviewByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reviews
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    
    public async Task DeleteAchievementAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        AchievementEntity? entity = await _dbContext.Achievements
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);

        if (entity is null)
        {
            throw new EntityNotFoundException(typeof(AchievementEntity), id);
        }

        _dbContext.Achievements.Remove(entity);
    }

    public async Task DeleteReviewAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        ReviewEntity? entity = await _dbContext.Reviews
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);

        if (entity is null)
        {
            throw new EntityNotFoundException(typeof(ReviewEntity), id);
        }

        _dbContext.Reviews.Remove(entity);
    }

    private static IQueryable<GameTitleEntity> ApplyFilter(
        IQueryable<GameTitleEntity> query,
        string? searchTerm,
        int? pegiRating,
        bool? isAvailable,
        string? publisher)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x => x.Name.Contains(searchTerm) || x.Description.Contains(searchTerm));
        }

        if (pegiRating.HasValue)
        {
            query = query.Where(x => x.PegiRating == pegiRating.Value);
        }

        if (isAvailable.HasValue)
        {
            query = query.Where(x => x.IsAvailable == isAvailable.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(publisher))
        {
            query = query.Where(x => x.Publisher.Contains(publisher));
        }
        
        return query;
    }

    private static IQueryable<GameTitleEntity> ApplySort(
        IQueryable<GameTitleEntity> query,
        GameTitleSortBy? sortBy,
        bool descending)
    {
        return (sortBy, descending) switch
        {
            (GameTitleSortBy.Name, false) => query.OrderBy(x => x.Name),
            (GameTitleSortBy.Name, true) => query.OrderByDescending(x => x.Name),

            (GameTitleSortBy.PegiRating, false) => query.OrderBy(x => x.PegiRating),
            (GameTitleSortBy.PegiRating, true) => query.OrderByDescending(x => x.PegiRating),

            (GameTitleSortBy.PriceCents, false) => query.OrderBy(x => x.PriceCents),
            (GameTitleSortBy.PriceCents, true) => query.OrderByDescending(x => x.PriceCents),

            (GameTitleSortBy.ReleaseDate, false) => query.OrderBy(x => x.ReleaseDate),
            (GameTitleSortBy.ReleaseDate, true) => query.OrderByDescending(x => x.ReleaseDate),

            _ => query.OrderBy(x => x.Name)
        };
    }
}