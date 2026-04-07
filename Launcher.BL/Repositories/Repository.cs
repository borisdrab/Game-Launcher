using Launcher.BL.Mappers.Interfaces;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.BL.Repositories;

public class Repository<TEntity>(LauncherDbContext ctx, IEntityMapper<TEntity> entityMapper)
    : IRepository<TEntity>
    where TEntity : class, IEntity
{
    private readonly DbSet<TEntity> _dbSet = ctx.Set<TEntity>();
    
    public IQueryable<TEntity> Get()
        => _dbSet.AsNoTracking();

    public TEntity Insert(TEntity entity)
        => _dbSet.Add(entity).Entity;

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        TEntity? existingEntity = await _dbSet
            .SingleOrDefaultAsync(e => e.Id == entity.Id, cancellationToken)
            .ConfigureAwait(false);
        if (existingEntity is null)
        {
            throw new EntityNotFoundException(typeof(TEntity), entity.Id);
        }
        
        entityMapper.MapToExistingEntity(existingEntity, entity);
        return existingEntity;
    }

    public async Task DeleteAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        TEntity? entity = await _dbSet
            .SingleOrDefaultAsync(e => e.Id == entityId, cancellationToken)
            .ConfigureAwait(false);
        if (entity is null)
        {
            throw new EntityNotFoundException(typeof(TEntity), entityId);
        }
        
        _dbSet.Remove(entity);
    }
    
    public async ValueTask<bool> ExistsAsync(TEntity entity, CancellationToken cancellationToken = default)
        => entity.Id != Guid.Empty
           && await _dbSet.AnyAsync(e => e.Id == entity.Id, cancellationToken).ConfigureAwait(false);
}