using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Helpers;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Models;
using Launcher.BL.Repositories.Interfaces;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.BL.Facades;

public class GenreFacade(
    LauncherDbContext ctx,
    IGenreRepository genreRepository,
    IModelMapper<GenreEntity, GenreListModel, GenreDetailModel> mapper)
    : FacadeBase<GenreEntity, GenreListModel, GenreDetailModel>(mapper), IGenreFacade
{
    public override async Task<IEnumerable<GenreListModel>> GetAsync()
        => _mapper.MapToListModel(await genreRepository.Get().ToListAsync());

    public async Task<IEnumerable<GenreListModel>> GetAsync(QueryObject query)
    {
        IQueryable<GenreEntity> dbQuery = genreRepository.Get();

        if (QueryHelper.HasSearchTerm(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(g => g.Name.Contains(query.SearchTerm!));
        }

        if (query.SortBy == "Name")
        {
            dbQuery = QueryHelper.ApplySort(dbQuery, g => g.Name, query.SortDescending);
        }

        var entities = await dbQuery.ToListAsync();
        return _mapper.MapToListModel(entities);
    }

    public override async Task<GenreDetailModel?> GetAsync(Guid id)
    {
        var entity = await genreRepository.Get().FirstOrDefaultAsync(g => g.Id == id);
        return entity is null ? null : _mapper.MapToDetailModel(entity);
    }

    public override async Task<Guid> SaveAsync(GenreDetailModel model)
    {
        var entity = _mapper.MapToEntity(model);

        if (model.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
            genreRepository.Insert(entity);
            await ctx.SaveChangesAsync();
            return entity.Id;
        }

        await genreRepository.UpdateAsync(entity);
        await ctx.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task DeleteAsync(Guid id)
    {
        await genreRepository.DeleteAsync(id);
        await ctx.SaveChangesAsync();
    }
}
