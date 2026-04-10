using Launcher.BL.Models;
using Launcher.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Launcher.BL.Mappers
{
    public class LibraryMapper
        : ModelMapperBase<LibraryEntity, LibraryListModel, LibraryDetailModel>
    {
        public override LibraryDetailModel MapToDetailModel(LibraryEntity? entity)
        => entity is null
            ? LibraryDetailModel.Empty
            : new LibraryDetailModel
            {
                Id = entity.Id,
                Name = entity.Name,
                LibraryTitles = entity.LibraryTitles,
                UserId = entity.UserId,
                User = entity.User,
            };

        public override LibraryEntity MapToEntity(LibraryDetailModel model)
        => new()
            {
                Id = model.Id,
                Name = model.Name,
                LibraryTitles = model.LibraryTitles,
                User = model.User,
                UserId = model.UserId,

            };

        public override LibraryListModel MapToListModel(LibraryEntity? entity)
        => entity is null
            ? LibraryListModel.Empty
            : new LibraryListModel
            {
                Id = entity.Id,
                Name = entity.Name,
                LibraryTitles = entity.LibraryTitles,
                User = entity.User,
                UserId = entity.UserId,
            };
    }
}
