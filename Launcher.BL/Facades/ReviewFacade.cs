using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Helpers;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Models;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.BL.Facades;

public class ReviewFacade
    : FacadeBase<ReviewEntity, ReviewListModel, ReviewDetailModel>,
        IReviewFacade
{
    private readonly IDbContextFactory<LauncherDbContext> _dbContextFactory;

    public ReviewFacade(
        IModelMapper<ReviewEntity, ReviewListModel, ReviewDetailModel> mapper,
        IDbContextFactory<LauncherDbContext> dbContextFactory)
        : base(mapper)
    {
        _dbContextFactory = dbContextFactory;
    }

    public override async Task<IEnumerable<ReviewListModel>> GetAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entities = await dbContext.Reviews
            .AsNoTracking()
            .ToListAsync();

        return _mapper.MapToListModel(entities);
    }

    public override async Task<IEnumerable<ReviewListModel>> GetAsync(QueryObject query)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Start building the database query
        IQueryable<ReviewEntity> dbQuery = dbContext.Reviews.AsNoTracking();

        // Search by review text if a search term was provided
        if (QueryHelper.HasSearchTerm(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(r => r.Text != null && r.Text.Contains(query.SearchTerm!));
        }

        // Sort by rating or by date
        if (query.SortBy == "Rating")
        {
            dbQuery = QueryHelper.ApplySort(dbQuery, r => r.Rating, query.SortDescending);
        }
        else if (query.SortBy == "CreatedAt")
        {
            dbQuery = QueryHelper.ApplySort(dbQuery, r => r.CreatedAt, query.SortDescending);
        }

        // Execute the query in the database and return results
        var entities = await dbQuery.ToListAsync();
        return _mapper.MapToListModel(entities);
    }

    public override async Task<ReviewDetailModel?> GetAsync(Guid id)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entity = await dbContext.Reviews
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (entity is null)
        {
            return null;
        }

        return _mapper.MapToDetailModel(entity);
    }

    public override async Task<Guid> SaveAsync(ReviewDetailModel model)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entity = _mapper.MapToEntity(model);

        var existingEntity = await dbContext.Reviews
            .FirstOrDefaultAsync(r => r.Id == entity.Id);

        if (existingEntity is null)
        {
            // New review
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            await dbContext.Reviews.AddAsync(entity);
        }
        else
        {
            // Update existing review
            existingEntity.Rating = entity.Rating;
            existingEntity.Text = entity.Text;
            existingEntity.UpdatedAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync();

        return entity.Id;
    }

    public override async Task DeleteAsync(Guid id)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var entity = await dbContext.Reviews
            .FirstOrDefaultAsync(r => r.Id == id);

        if (entity is not null)
        {
            dbContext.Reviews.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
