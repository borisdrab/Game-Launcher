using Launcher.BL.Models;

namespace Launcher.BL.Facades.Interfaces;

public interface IGenreFacade
    : IFacade<GenreListModel, GenreDetailModel>
{
    Task<IEnumerable<GenreListModel>> GetAsync(QueryObject query);
}
