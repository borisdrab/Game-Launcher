using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers.Interfaces;

public interface IEntityMapper<in TEntity>
    where TEntity : IEntity 
{
    void MapToExistingEntity(TEntity existingEntity, TEntity newEntity);
}