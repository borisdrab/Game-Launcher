using Launcher.DAL.Entities;
using Launcher.DAL.Context;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Repositories.Interfaces;

namespace Launcher.BL.Repositories;

public class GameTitleRepository : Repository<GameTitleEntity>, IGameTitleRepository
{
    public GameTitleRepository(
        LauncherDbContext dbContext,
        IEntityMapper<GameTitleEntity> entityMapper)
        : base(dbContext, entityMapper)
    {
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