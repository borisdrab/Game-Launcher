using Launcher.BL.Mappers.Interfaces;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class UserEntityMapper : IEntityMapper<UserEntity>
{
    public void MapToExistingEntity(UserEntity existingEntity, UserEntity newEntity)
    {
        existingEntity.UserName = newEntity.UserName;
        existingEntity.Email = newEntity.Email;
        existingEntity.DisplayName = newEntity.DisplayName;
        existingEntity.AvatarUrl = newEntity.AvatarUrl;
    }
}