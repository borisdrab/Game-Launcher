using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Models;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.BL.Facades;

public class GenreFacade
    : FacadeBase<GenreEntity, GenreListModel, GenreDetailModel>,
        IGenreFacade
{
    private readonly IDbContextFactory<LauncherDbContext> _dbContextFactory;

    public GenreFacade(
        IModelMapper<GenreEntity, GenreListModel, GenreDetailModel> mapper,
        IDbContextFactory<LauncherDbContext> dbContextFactory)
        : base(mapper)
    {
        _dbContextFactory = dbContextFactory;
    }

    public override async Task<IEnumerable<GenreListModel>> GetAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entities = await dbContext.Genres
            .AsNoTracking()
            .ToListAsync();

        return _mapper.MapToListModel(entities);
    }

    public override async Task<IEnumerable<GenreListModel>> GetAsync(QueryObject query)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        
        IQueryable<GenreEntity> dbQuery = dbContext.Genres.AsNoTracking();
        
        if (string.IsNullOrWhiteSpace(query.SearchTerm) == false)
        {
            dbQuery = dbQuery.Where(g => g.Name.Contains(query.SearchTerm));
        }
        
        if (query.SortBy == "Name")
        {
            if (query.SortDescending)
            {
                dbQuery = dbQuery.OrderByDescending(g => g.Name);
            }
            else
            {
                dbQuery = dbQuery.OrderBy(g => g.Name);
            }
        }
        
        var entities = await dbQuery.ToListAsync();
        return _mapper.MapToListModel(entities);
    }

    public override async Task<GenreDetailModel?> GetAsync(Guid id)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entity = await dbContext.Genres
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);

        if (entity is null)
        {
            return null;
        }

        return _mapper.MapToDetailModel(entity);
    }

    public override async Task<Guid> SaveAsync(GenreDetailModel model)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entity = _mapper.MapToEntity(model);

        var existingEntity = await dbContext.Genres
            .FirstOrDefaultAsync(g => g.Id == entity.Id);

        if (existingEntity is null)
        {
            entity.Id = Guid.NewGuid();
            await dbContext.Genres.AddAsync(entity);
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

        var entity = await dbContext.Genres
            .FirstOrDefaultAsync(g => g.Id == id);

        if (entity is not null)
        {
            dbContext.Genres.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
