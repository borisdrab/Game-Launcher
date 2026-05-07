using Launcher.BL.Mappers.Interfaces;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class GenreEntityMapper : IEntityMapper<GenreEntity>
{
    public void MapToExistingEntity(GenreEntity existingEntity, GenreEntity newEntity)
    {
        existingEntity.Name = newEntity.Name;
    }
}
