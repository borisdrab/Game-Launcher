using Launcher.BL.Models;
using Launcher.BL.Repositories;

namespace Launcher.BL.Facades.Interfaces;

public interface IGameTitleFacade
    : IFacade<GameTitleListModel, GameTitleDetailModel>
{
    Task<IEnumerable<GameTitleListModel>> GetAsync(
        string? searchTerm,
        int? pegiRating,
        bool? isAvailable,
        string? publisher,
        GameTitleSortBy? sortBy,
        bool descending);

    Task AddGenreAsync(Guid gameTitleId, Guid genreId);
    Task RemoveGenreAsync(Guid gameTitleId, Guid genreId);

    Task AddPlatformAsync(Guid gameTitleId, Guid platformId);
    Task RemovePlatformAsync(Guid gameTitleId, Guid platformId);

    Task AddAchievementAsync(Guid gameTitleId, AchievementDetailModel model);
    Task UpdateAchievementAsync(Guid gameTitleId, AchievementDetailModel model);
    Task RemoveAchievementAsync(Guid achievementId);

    Task AddReviewAsync(Guid gameTitleId, ReviewDetailModel model);
    Task UpdateReviewAsync(Guid gameTitleId, ReviewDetailModel model);
    Task RemoveReviewAsync(Guid reviewId);
}