using Launcher.BL.Models;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class GameTitleModelMapper
    : ModelMapperBase<GameTitleEntity, GameTitleListModel, GameTitleDetailModel>
{
    public override GameTitleListModel MapToListModel(GameTitleEntity? entity)
        => entity is null
            ? GameTitleListModel.Empty
            : new GameTitleListModel
            {
                Id = entity.Id,
                Name = entity.Name,
                PegiRating = entity.PegiRating,
                PriceCents = entity.PriceCents,
                CoverImageUrl = entity.CoverImageUrl,
                Publisher = entity.Publisher,
                ReleaseDate = entity.ReleaseDate,
                IsAvailable = entity.IsAvailable
            };

    public override GameTitleDetailModel MapToDetailModel(GameTitleEntity? entity)
        => entity is null
            ? GameTitleDetailModel.Empty
            : new GameTitleDetailModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                PegiRating = entity.PegiRating,
                PriceCents = entity.PriceCents,
                CoverImageUrl = entity.CoverImageUrl,
                Publisher = entity.Publisher,
                ReleaseDate = entity.ReleaseDate,
                IsAvailable = entity.IsAvailable
            };

    public override GameTitleEntity MapToEntity(GameTitleDetailModel model)
        => new()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            PegiRating = model.PegiRating,
            PriceCents = model.PriceCents,
            CoverImageUrl = model.CoverImageUrl,
            Publisher = model.Publisher,
            ReleaseDate = model.ReleaseDate,
            IsAvailable = model.IsAvailable
        };
}