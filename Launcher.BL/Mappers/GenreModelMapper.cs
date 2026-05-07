using Launcher.BL.Models;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class GenreModelMapper
    : ModelMapperBase<GenreEntity, GenreListModel, GenreDetailModel>
{
    public override GenreListModel MapToListModel(GenreEntity? entity)
        => entity is null
            ? GenreListModel.Empty
            : new GenreListModel
            {
                Id = entity.Id,
                Name = entity.Name
            };

    public override GenreDetailModel MapToDetailModel(GenreEntity? entity)
        => entity is null
            ? GenreDetailModel.Empty
            : new GenreDetailModel
            {
                Id = entity.Id,
                Name = entity.Name
            };

    public override GenreEntity MapToEntity(GenreDetailModel model)
        => new()
        {
            Id = model.Id,
            Name = model.Name
        };
}
