using Launcher.BL.Models;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class ReviewModelMapper
    : ModelMapperBase<ReviewEntity, ReviewListModel, ReviewDetailModel>
{
    public override ReviewListModel MapToListModel(ReviewEntity? entity)
    {
        if (entity is null)
        {
            return ReviewListModel.Empty;
        }

        var model = new ReviewListModel();
        model.Id = entity.Id;
        model.UserId = entity.UserId;
        model.GameTitleId = entity.GameTitleId;
        model.Rating = entity.Rating;
        model.CreatedAt = entity.CreatedAt;
        return model;
    }

    public override ReviewDetailModel MapToDetailModel(ReviewEntity? entity)
    {
        if (entity is null)
        {
            return ReviewDetailModel.Empty;
        }

        var model = new ReviewDetailModel();
        model.Id = entity.Id;
        model.UserId = entity.UserId;
        model.GameTitleId = entity.GameTitleId;
        model.Rating = entity.Rating;
        model.Text = entity.Text;
        model.CreatedAt = entity.CreatedAt;
        model.UpdatedAt = entity.UpdatedAt;
        return model;
    }

    public override ReviewEntity MapToEntity(ReviewDetailModel model)
    {
        var entity = new ReviewEntity();
        entity.Id = model.Id;
        entity.UserId = model.UserId;
        entity.GameTitleId = model.GameTitleId;
        entity.Rating = model.Rating;
        entity.Text = model.Text;
        entity.CreatedAt = model.CreatedAt;
        entity.UpdatedAt = model.UpdatedAt;
        return entity;
    }
}
