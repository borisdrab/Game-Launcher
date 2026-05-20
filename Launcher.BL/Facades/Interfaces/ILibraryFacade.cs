using System;
using System.Threading.Tasks;
using Launcher.BL.Models;

namespace Launcher.BL.Facades.Interfaces;

public interface ILibraryFacade : IFacade<LibraryListModel, LibraryDetailModel>
{
    Task<LibraryListModel> FilterAsync(Guid userId, string? gameName, string? sortBy, bool ascending, params string[] genres);
    Task<bool> IsGameInLibraryAsync(Guid userId, Guid gameTitleId);
    Task AddGameToLibraryAsync(Guid userId, Guid gameTitleId);
    Task ToggleFavoriteAsync(Guid libraryId, Guid gameTitleId);
}
