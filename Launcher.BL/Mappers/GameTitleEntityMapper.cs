using Launcher.DAL.Entities;
using Launcher.BL.Mappers.Interfaces;

namespace Launcher.BL.Mappers;

public class GameTitleEntityMapper : IEntityMapper<GameTitleEntity>
{
    public void MapToExistingEntity(GameTitleEntity existingEntity, GameTitleEntity newEntity)
    {
        existingEntity.Name = newEntity.Name;
        existingEntity.Description = newEntity.Description;
        existingEntity.PegiRating = newEntity.PegiRating;
        existingEntity.PriceCents = newEntity.PriceCents;
        existingEntity.CoverImageUrl = newEntity.CoverImageUrl;
        existingEntity.Publisher = newEntity.Publisher;
        existingEntity.ReleaseDate = newEntity.ReleaseDate;
        existingEntity.IsAvailable = newEntity.IsAvailable;
    }
}