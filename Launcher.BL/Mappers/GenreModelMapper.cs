using Launcher.BL.Models;
using Launcher.DAL.Entities;

namespace Launcher.BL.Mappers;

public class GenreModelMapper
    : ModelMapperBase<GenreEntity, GenreListModel, GenreDetailModel>
{
    public override GenreListModel MapToListModel(GenreEntity? entity)
    {
        if (entity is null)
        {
            return GenreListModel.Empty;
        }

        var model = new GenreListModel();
        model.Id = entity.Id;
        model.Name = entity.Name;
        return model;
    }

    public override GenreDetailModel MapToDetailModel(GenreEntity? entity)
    {
        if (entity is null)
        {
            return GenreDetailModel.Empty;
        }

        var model = new GenreDetailModel();
        model.Id = entity.Id;
        model.Name = entity.Name;
        return model;
    }

    public override GenreEntity MapToEntity(GenreDetailModel model)
    {
        var entity = new GenreEntity();
        entity.Id = model.Id;
        entity.Name = model.Name;
        return entity;
    }
}
