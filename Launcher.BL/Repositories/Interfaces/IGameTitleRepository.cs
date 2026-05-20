using Launcher.DAL.Entities;

namespace Launcher.BL.Repositories.Interfaces;

public interface IGameTitleRepository :  IRepository<GameTitleEntity>
{
    IQueryable<GameTitleEntity> GetQuery(
        string? searchTerm,
        int? pegiRating,
        bool? isAvailable,
        string? publisher,
        IEnumerable<Guid>? genreIds,
        GameTitleSortBy? sortBy,
        bool descending);
}