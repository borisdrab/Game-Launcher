using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Helpers;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Models;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.BL.Facades;

public class AchievementFacade
    : FacadeBase<AchievementEntity, AchievementListModel, AchievementDetailModel>,
        IAchievementFacade
{
    private readonly IDbContextFactory<LauncherDbContext> _dbContextFactory;

    public AchievementFacade(
        IModelMapper<AchievementEntity, AchievementListModel, AchievementDetailModel> mapper,
        IDbContextFactory<LauncherDbContext> dbContextFactory)
        : base(mapper)
    {
        _dbContextFactory = dbContextFactory;
    }

    public override async Task<IEnumerable<AchievementListModel>> GetAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entities = await dbContext.Achievements
            .AsNoTracking()
            .ToListAsync();

        return _mapper.MapToListModel(entities);
    }

    public async Task<IEnumerable<AchievementListModel>> GetAsync(QueryObject query)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Start building the database query
        IQueryable<AchievementEntity> dbQuery = dbContext.Achievements.AsNoTracking();

        // Search by name if a search term was provided
        if (QueryHelper.HasSearchTerm(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(a => a.Name.Contains(query.SearchTerm!));
        }

        // Sort by name or by points
        if (query.SortBy == "Name")
        {
            dbQuery = QueryHelper.ApplySort(dbQuery, a => a.Name, query.SortDescending);
        }
        else if (query.SortBy == "Points")
        {
            dbQuery = QueryHelper.ApplySort(dbQuery, a => a.Points, query.SortDescending);
        }

        // Execute the query in the database and return results
        var entities = await dbQuery.ToListAsync();
        return _mapper.MapToListModel(entities);
    }

    public override async Task<AchievementDetailModel?> GetAsync(Guid id)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entity = await dbContext.Achievements
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (entity is null)
        {
            return null;
        }

        return _mapper.MapToDetailModel(entity);
    }

    public override async Task<Guid> SaveAsync(AchievementDetailModel model)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entity = _mapper.MapToEntity(model);

        var existingEntity = await dbContext.Achievements
            .FirstOrDefaultAsync(a => a.Id == entity.Id);

        if (existingEntity is null)
        {
            entity.Id = Guid.NewGuid();
            await dbContext.Achievements.AddAsync(entity);
        }
        else
        {
            existingEntity.Name = entity.Name;
            existingEntity.Description = entity.Description;
            existingEntity.Points = entity.Points;
        }

        await dbContext.SaveChangesAsync();

        return entity.Id;
    }

    public override async Task DeleteAsync(Guid id)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entity = await dbContext.Achievements
            .FirstOrDefaultAsync(a => a.Id == id);

        if (entity is not null)
        {
            dbContext.Achievements.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
