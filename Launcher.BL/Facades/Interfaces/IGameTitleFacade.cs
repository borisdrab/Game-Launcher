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
        IEnumerable<Guid>? genreIds,
        GameTitleSortBy? sortBy,
        bool descending);
    
    Task AddGenreAsync(Guid gameTitleId, Guid genreId);
    Task RemoveGenreAsync(Guid gameTitleId, Guid genreId);

    Task AddPlatformAsync(Guid gameTitleId, Guid platformId);
    Task RemovePlatformAsync(Guid gameTitleId, Guid platformId);
}