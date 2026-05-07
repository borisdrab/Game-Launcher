using Launcher.BL.Models;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class PlatformModelMapper
    : ModelMapperBase<PlatformEntity, PlatformListModel, PlatformDetailModel>
{
    public override PlatformListModel MapToListModel(PlatformEntity? entity)
        => entity is null
            ? PlatformListModel.Empty
            : new PlatformListModel
            {
                Id = entity.Id,
                Name = entity.Name
            };

    public override PlatformDetailModel MapToDetailModel(PlatformEntity? entity)
        => entity is null
            ? PlatformDetailModel.Empty
            : new PlatformDetailModel
            {
                Id = entity.Id,
                Name = entity.Name
            };

    public override PlatformEntity MapToEntity(PlatformDetailModel model)
        => new()
        {
            Id = model.Id,
            Name = model.Name
        };
}
