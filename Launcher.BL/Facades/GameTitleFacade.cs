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
            sortBy,
            query.SortDescending);
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
}