using Launcher.BL.Models;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class AchievementModelMapper
    : ModelMapperBase<AchievementEntity, AchievementListModel, AchievementDetailModel>
{
    public override AchievementListModel MapToListModel(AchievementEntity? entity)
    {
        if (entity is null)
        {
            return AchievementListModel.Empty;
        }

        var model = new AchievementListModel();
        model.Id = entity.Id;
        model.Name = entity.Name;
        model.Points = entity.Points;
        model.GameTitleId = entity.GameTitleId;
        return model;
    }

    public override AchievementDetailModel MapToDetailModel(AchievementEntity? entity)
    {
        if (entity is null)
        {
            return AchievementDetailModel.Empty;
        }

        var model = new AchievementDetailModel();
        model.Id = entity.Id;
        model.Name = entity.Name;
        model.Description = entity.Description;
        model.Points = entity.Points;
        model.GameTitleId = entity.GameTitleId;
        return model;
    }

    public override AchievementEntity MapToEntity(AchievementDetailModel model)
    {
        var entity = new AchievementEntity();
        entity.Id = model.Id;
        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.Points = model.Points;
        entity.GameTitleId = model.GameTitleId;
        return entity;
    }
}
