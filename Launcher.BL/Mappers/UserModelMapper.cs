using Launcher.BL.Models;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class UserModelMapper
    : ModelMapperBase<UserEntity, UserListModel, UserDetailModel>
{
    public override UserListModel MapToListModel(UserEntity? entity)
        => entity is null
            ? UserListModel.Empty
            : new UserListModel
            {
                Id = entity.Id,
                UserName = entity.UserName,
                DisplayName = entity.DisplayName,
                AvatarUrl = entity.AvatarUrl,
            };

    public override UserDetailModel MapToDetailModel(UserEntity? entity)
        => entity is null
            ? UserDetailModel.Empty
            : new UserDetailModel
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Email = entity.Email,
                DisplayName = entity.DisplayName,
                AvatarUrl = entity.AvatarUrl,
            };

    public override UserEntity MapToEntity(UserDetailModel model)
        => new()
        {
            Id = model.Id,
            UserName = model.UserName,
            Email = model.Email,
            DisplayName = model.DisplayName,
            AvatarUrl = model.AvatarUrl,
        };
}