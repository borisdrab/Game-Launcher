using Launcher.BL.Mappers.Interfaces;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class PlatformEntityMapper : IEntityMapper<PlatformEntity>
{
    public void MapToExistingEntity(PlatformEntity existingEntity, PlatformEntity newEntity)
    {
        existingEntity.Name = newEntity.Name;
    }
}
