using Launcher.DAL.Entities;

namespace Launcher.BL.Repositories.Interfaces;

public interface IGameTitleRepository :  IRepository<GameTitleEntity>
{
    IQueryable<GameTitleEntity> GetQuery(
        string? searchTerm,
        int? pegiRating,
        bool? isAvailable,
        string? publisher,
        GameTitleSortBy? sortBy,
        bool descending);
    
    Task<GameTitleEntity?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<AchievementEntity?> GetAchievementByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReviewEntity?> GetReviewByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task DeleteAchievementAsync(Guid id, CancellationToken cancellationToken = default);

    Task DeleteReviewAsync(Guid id, CancellationToken cancellationToken = default);
}