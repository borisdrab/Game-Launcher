using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Mappers;
using Launcher.BL.Models;
using Launcher.BL.Repositories;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.BL.Facades;

public class GameTitleFacade(
    IDbContextFactory<LauncherDbContext> dbContextFactory,
    GameTitleModelMapper mapper)
    : FacadeBase<GameTitleEntity, GameTitleListModel, GameTitleDetailModel>(mapper), IGameTitleFacade
{
    public override async Task<IEnumerable<GameTitleListModel>> GetAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var entities = await dbContext.GameTitles
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync();

        return mapper.MapToListModel(entities);
    }

    public async Task<IEnumerable<GameTitleListModel>> GetAsync(
        string? searchTerm,
        int? pegiRating,
        bool? isAvailable,
        string? publisher,
        IEnumerable<Guid>? genreIds,
        GameTitleSortBy? sortBy,
        bool descending)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        IQueryable<GameTitleEntity> query = dbContext.GameTitles
            .AsNoTracking()
            .Include(x => x.GameTitleGenres);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string loweredSearch = searchTerm.ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(loweredSearch) ||
                x.Description.ToLower().Contains(loweredSearch));
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
        
        var genreIdSet = genreIds?.ToHashSet();

        if (genreIdSet is not null && genreIdSet.Count > 0)
        {
            query = query.Where(x => x.GameTitleGenres.Any(gtg => genreIdSet.Contains(gtg.GenreId)));
        }

        query = (sortBy, descending) switch
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

        var entities = await query.ToListAsync();
        return mapper.MapToListModel(entities);
    }

    public Task<IEnumerable<GameTitleListModel>> GetAsync(QueryObject query)
    {
        GameTitleSortBy? sortBy = query.SortBy?.ToLower() switch
        {
            "name" => GameTitleSortBy.Name,
            "pegirating" => GameTitleSortBy.PegiRating,
            "pricecents" => GameTitleSortBy.PriceCents,
            "releasedate" => GameTitleSortBy.ReleaseDate,
            _ => null
        };

        return GetAsync(
            query.SearchTerm,
            null,
            null,
            null,
            null,
            sortBy,
            query.SortDescending);
    }

    public override async Task<GameTitleDetailModel?> GetAsync(Guid id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var entity = await dbContext.GameTitles
            .AsNoTracking()
            .Include(x => x.GameTitleGenres)
                .ThenInclude(x => x.Genre)
            .Include(x => x.GameTitlePlatforms)
                .ThenInclude(x => x.Platform)
            .Include(x => x.Reviews)
                .ThenInclude(x => x.User)
            .Include(x => x.Achievements)
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity is null ? null : mapper.MapToDetailModel(entity);
    }

    public override async Task<Guid> SaveAsync(GameTitleDetailModel model)
    {
        if (model.Genres.Count > 0 ||
            model.Platforms.Count > 0 ||
            model.Achievements.Count > 0 ||
            model.Reviews.Count > 0)
        {
            throw new InvalidOperationException("SaveAsync supports only scalar GameTitle properties.");
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var entity = mapper.MapToEntity(model);

        if (model.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
            await dbContext.GameTitles.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return entity.Id;
        }

        var existingEntity = await dbContext.GameTitles
            .FirstOrDefaultAsync(x => x.Id == model.Id);

        if (existingEntity is null)
        {
            throw new InvalidOperationException($"GameTitle with ID {model.Id} was not found.");
        }

        existingEntity.Name = entity.Name;
        existingEntity.Description = entity.Description;
        existingEntity.PegiRating = entity.PegiRating;
        existingEntity.PriceCents = entity.PriceCents;
        existingEntity.CoverImageUrl = entity.CoverImageUrl;
        existingEntity.Publisher = entity.Publisher;
        existingEntity.ReleaseDate = entity.ReleaseDate;
        existingEntity.IsAvailable = entity.IsAvailable;

        await dbContext.SaveChangesAsync();
        return existingEntity.Id;
    }

    public override async Task DeleteAsync(Guid id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var entity = await dbContext.GameTitles.FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null)
        {
            throw new InvalidOperationException($"GameTitle with ID {id} was not found.");
        }

        dbContext.GameTitles.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddGenreAsync(Guid gameTitleId, Guid genreId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        bool gameExists = await dbContext.GameTitles.AnyAsync(x => x.Id == gameTitleId);
        if (!gameExists)
        {
            throw new InvalidOperationException($"GameTitle with id {gameTitleId} was not found.");
        }

        bool relationExists = await dbContext.Set<GameTitleGenreEntity>()
            .AnyAsync(x => x.GameTitleId == gameTitleId && x.GenreId == genreId);

        if (relationExists)
        {
            return;
        }

        dbContext.Set<GameTitleGenreEntity>().Add(mapper.MapGenreToEntity(gameTitleId, genreId));
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveGenreAsync(Guid gameTitleId, Guid genreId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        GameTitleGenreEntity? relation = await dbContext.Set<GameTitleGenreEntity>()
            .SingleOrDefaultAsync(x => x.GameTitleId == gameTitleId && x.GenreId == genreId);

        if (relation is null)
        {
            return;
        }

        dbContext.Set<GameTitleGenreEntity>().Remove(relation);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddPlatformAsync(Guid gameTitleId, Guid platformId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        bool gameExists = await dbContext.GameTitles.AnyAsync(x => x.Id == gameTitleId);
        if (!gameExists)
        {
            throw new InvalidOperationException($"GameTitle with id {gameTitleId} was not found.");
        }

        bool relationExists = await dbContext.Set<GameTitlePlatformEntity>()
            .AnyAsync(x => x.GameTitleId == gameTitleId && x.PlatformId == platformId);

        if (relationExists)
        {
            return;
        }

        dbContext.Set<GameTitlePlatformEntity>().Add(mapper.MapPlatformToEntity(gameTitleId, platformId));
        await dbContext.SaveChangesAsync();
    }

    public async Task RemovePlatformAsync(Guid gameTitleId, Guid platformId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        GameTitlePlatformEntity? relation = await dbContext.Set<GameTitlePlatformEntity>()
            .SingleOrDefaultAsync(x => x.GameTitleId == gameTitleId && x.PlatformId == platformId);

        if (relation is null)
        {
            return;
        }

        dbContext.Set<GameTitlePlatformEntity>().Remove(relation);
        await dbContext.SaveChangesAsync();
    }
}