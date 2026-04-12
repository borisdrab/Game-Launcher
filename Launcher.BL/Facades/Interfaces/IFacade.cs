namespace Launcher.BL.Facades.Interfaces;

public interface IFacade<TListModel, TDetailModel>
{
    Task<IEnumerable<TListModel>> GetAsync();

    Task<TDetailModel?> GetAsync(Guid id);

    Task<Guid> SaveAsync(TDetailModel model);

    Task DeleteAsync(Guid id);
}
