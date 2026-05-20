using Launcher.BL.Models;
using Launcher.DAL.Entities;
using System;
using System.Linq;

namespace Launcher.BL.Mappers;

public class LibraryModelMapper
    : ModelMapperBase<LibraryEntity, LibraryListModel, LibraryDetailModel>
{
    public override LibraryDetailModel MapToDetailModel(LibraryEntity? entity)
    => entity is null
        ? LibraryDetailModel.Empty
        : new LibraryDetailModel
        {
            Id = entity.Id,
            Name = entity.Name,
            UserId = entity.UserId,
            User = entity.User is null ? null : new UserDetailModel
            {
                Id = entity.User.Id,
                UserName = entity.User.UserName,
                Email = entity.User.Email,
                DisplayName = entity.User.DisplayName,
                AvatarUrl = entity.User.AvatarUrl
            },
            LibraryTitles = entity.LibraryTitles.Select(lt => new LibraryTitleListModel
            {
                LibraryId = lt.LibraryId,
                GameTitleId = lt.GameTitleId,
                AddedAt = lt.AddedAt,
                IsFavorite = lt.IsFavorite,
                PriceCentsAtPurchase = lt.PriceCentsAtPurchase,
                GameTitle = lt.GameTitle is null ? null : new GameTitleListModel
                {
                    Id = lt.GameTitle.Id,
                    Name = lt.GameTitle.Name,
                    PegiRating = lt.GameTitle.PegiRating,
                    PriceCents = lt.GameTitle.PriceCents,
                    CoverImageUrl = lt.GameTitle.CoverImageUrl,
                    Publisher = lt.GameTitle.Publisher,
                    ReleaseDate = lt.GameTitle.ReleaseDate,
                    IsAvailable = lt.GameTitle.IsAvailable
                }
            }).ToList()
        };

    public override LibraryEntity MapToEntity(LibraryDetailModel model)
    => new()
    {
        Id = model.Id,
        Name = model.Name,
        UserId = model.UserId,
    };

    public override LibraryListModel MapToListModel(LibraryEntity? entity)
    => entity is null
        ? LibraryListModel.Empty
        : new LibraryListModel
        {
            Id = entity.Id,
            Name = entity.Name,
            UserId = entity.UserId,
            User = entity.User is null ? null : new UserListModel
            {
                Id = entity.User.Id,
                UserName = entity.User.UserName,
                DisplayName = entity.User.DisplayName,
                AvatarUrl = entity.User.AvatarUrl
            },
            LibraryTitles = entity.LibraryTitles.Select(lt => new LibraryTitleListModel
            {
                LibraryId = lt.LibraryId,
                GameTitleId = lt.GameTitleId,
                AddedAt = lt.AddedAt,
                IsFavorite = lt.IsFavorite,
                PriceCentsAtPurchase = lt.PriceCentsAtPurchase,
                GameTitle = lt.GameTitle is null ? null : new GameTitleListModel
                {
                    Id = lt.GameTitle.Id,
                    Name = lt.GameTitle.Name,
                    PegiRating = lt.GameTitle.PegiRating,
                    PriceCents = lt.GameTitle.PriceCents,
                    CoverImageUrl = lt.GameTitle.CoverImageUrl,
                    Publisher = lt.GameTitle.Publisher,
                    ReleaseDate = lt.GameTitle.ReleaseDate,
                    IsAvailable = lt.GameTitle.IsAvailable
                }
            }).ToList()
        };
}
