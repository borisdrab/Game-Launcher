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
}