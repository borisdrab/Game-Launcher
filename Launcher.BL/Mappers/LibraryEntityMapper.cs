using Launcher.BL.Mappers.Interfaces;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class LibraryEntityMapper : IEntityMapper<LibraryEntity>
{
    public void MapToExistingEntity(LibraryEntity existingEntity, LibraryEntity newEntity)
    {
        existingEntity.Name = newEntity.Name;
        existingEntity.UserId = newEntity.UserId;
    }
}
