using Launcher.BL.Models;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class ReviewModelMapper
    : ModelMapperBase<ReviewEntity, ReviewListModel, ReviewDetailModel>
{
    public override ReviewListModel MapToListModel(ReviewEntity? entity)
        => entity is null
            ? ReviewListModel.Empty
            : new ReviewListModel
            {
                Id = entity.Id,
                UserId = entity.UserId,
                GameTitleId = entity.GameTitleId,
                Rating = entity.Rating,
                CreatedAt = entity.CreatedAt
            };

    public override ReviewDetailModel MapToDetailModel(ReviewEntity? entity)
        => entity is null
            ? ReviewDetailModel.Empty
            : new ReviewDetailModel
            {
                Id = entity.Id,
                UserId = entity.UserId,
                GameTitleId = entity.GameTitleId,
                Rating = entity.Rating,
                Text = entity.Text,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

    public override ReviewEntity MapToEntity(ReviewDetailModel model)
        => new()
        {
            Id = model.Id,
            UserId = model.UserId,
            GameTitleId = model.GameTitleId,
            Rating = model.Rating,
            Text = model.Text,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
}
