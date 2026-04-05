using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Models;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.BL.Facades;

public class PlatformFacade
    : FacadeBase<PlatformEntity, PlatformListModel, PlatformDetailModel>,
        IPlatformFacade
{
    private readonly IDbContextFactory<LauncherDbContext> _dbContextFactory;

    public PlatformFacade(
        IModelMapper<PlatformEntity, PlatformListModel, PlatformDetailModel> mapper,
        IDbContextFactory<LauncherDbContext> dbContextFactory)
        : base(mapper)
    {
        _dbContextFactory = dbContextFactory;
    }

    public override async Task<IEnumerable<PlatformListModel>> GetAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entities = await dbContext.Platforms
            .AsNoTracking()
            .ToListAsync();

        return _mapper.MapToListModel(entities);
    }

    public override async Task<PlatformDetailModel?> GetAsync(Guid id)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entity = await dbContext.Platforms
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entity is null)
        {
            return null;
        }

        return _mapper.MapToDetailModel(entity);
    }

    public override async Task<Guid> SaveAsync(PlatformDetailModel model)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entity = _mapper.MapToEntity(model);

        var existingEntity = await dbContext.Platforms
            .FirstOrDefaultAsync(p => p.Id == entity.Id);

        if (existingEntity is null)
        {
            entity.Id = Guid.NewGuid();
            await dbContext.Platforms.AddAsync(entity);
        }
        else
        {
            existingEntity.Name = entity.Name;
        }

        await dbContext.SaveChangesAsync();

        return entity.Id;
    }

    public override async Task DeleteAsync(Guid id)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entity = await dbContext.Platforms
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entity is not null)
        {
            dbContext.Platforms.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
