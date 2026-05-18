using Launcher.BL.Models;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class AchievementModelMapper
    : ModelMapperBase<AchievementEntity, AchievementListModel, AchievementDetailModel>
{
    public override AchievementListModel MapToListModel(AchievementEntity? entity)
        => entity is null
            ? AchievementListModel.Empty
            : new AchievementListModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Points = entity.Points,
                GameTitleId = entity.GameTitleId
            };

    public override AchievementDetailModel MapToDetailModel(AchievementEntity? entity)
        => entity is null
            ? AchievementDetailModel.Empty
            : new AchievementDetailModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Points = entity.Points,
                GameTitleId = entity.GameTitleId
            };

    public override AchievementEntity MapToEntity(AchievementDetailModel model)
        => new()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Points = model.Points,
            GameTitleId = model.GameTitleId
        };
}
