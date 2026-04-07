using Launcher.DAL.Entities;

namespace Launcher.BL.Repositories;

public interface IRepository<TEntity>
    where TEntity : class, IEntity
{
    IQueryable<TEntity> Get();
    TEntity Insert(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid entityId, CancellationToken cancellationToken = default);
    ValueTask<bool> ExistsAsync(TEntity entity, CancellationToken cancellationToken = default);
}