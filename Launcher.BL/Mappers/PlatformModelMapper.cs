using Launcher.BL.Models;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class PlatformModelMapper
    : ModelMapperBase<PlatformEntity, PlatformListModel, PlatformDetailModel>
{
    public override PlatformListModel MapToListModel(PlatformEntity? entity)
    {
        if (entity is null)
        {
            return PlatformListModel.Empty;
        }

        var model = new PlatformListModel();
        model.Id = entity.Id;
        model.Name = entity.Name;
        return model;
    }

    public override PlatformDetailModel MapToDetailModel(PlatformEntity? entity)
    {
        if (entity is null)
        {
            return PlatformDetailModel.Empty;
        }

        var model = new PlatformDetailModel();
        model.Id = entity.Id;
        model.Name = entity.Name;
        return model;
    }

    public override PlatformEntity MapToEntity(PlatformDetailModel model)
    {
        var entity = new PlatformEntity();
        entity.Id = model.Id;
        entity.Name = model.Name;
        return entity;
    }
}
