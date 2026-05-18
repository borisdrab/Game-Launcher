using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Helpers;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Models;
using Launcher.BL.Repositories.Interfaces;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.BL.Facades;

public class PlatformFacade(
    LauncherDbContext ctx,
    IPlatformRepository platformRepository,
    IModelMapper<PlatformEntity, PlatformListModel, PlatformDetailModel> mapper)
    : FacadeBase<PlatformEntity, PlatformListModel, PlatformDetailModel>(mapper), IPlatformFacade
{
    public override async Task<IEnumerable<PlatformListModel>> GetAsync()
        => _mapper.MapToListModel(await platformRepository.Get().ToListAsync());

    public async Task<IEnumerable<PlatformListModel>> GetAsync(QueryObject query)
    {
        IQueryable<PlatformEntity> dbQuery = platformRepository.Get();

        if (QueryHelper.HasSearchTerm(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(p => p.Name.Contains(query.SearchTerm!));
        }

        if (query.SortBy == "Name")
        {
            dbQuery = QueryHelper.ApplySort(dbQuery, p => p.Name, query.SortDescending);
        }

        var entities = await dbQuery.ToListAsync();
        return _mapper.MapToListModel(entities);
    }

    public override async Task<PlatformDetailModel?> GetAsync(Guid id)
    {
        var entity = await platformRepository.Get().FirstOrDefaultAsync(p => p.Id == id);
        return entity is null ? null : _mapper.MapToDetailModel(entity);
    }

    public override async Task<Guid> SaveAsync(PlatformDetailModel model)
    {
        var entity = _mapper.MapToEntity(model);

        if (model.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
            platformRepository.Insert(entity);
            await ctx.SaveChangesAsync();
            return entity.Id;
        }

        await platformRepository.UpdateAsync(entity);
        await ctx.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task DeleteAsync(Guid id)
    {
        await platformRepository.DeleteAsync(id);
        await ctx.SaveChangesAsync();
    }
}
