using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Facades.Interfaces;

namespace Launcher.BL.Facades;

public abstract class FacadeBase<TEntity, TListModel, TDetailModel>
    : IFacade<TListModel, TDetailModel>
{
    protected readonly IModelMapper<TEntity, TListModel, TDetailModel> _mapper;

    protected FacadeBase(
        IModelMapper<TEntity, TListModel, TDetailModel> mapper)
    {
        _mapper = mapper;
    }

    public abstract Task<IEnumerable<TListModel>> GetAsync();

    public abstract Task<TDetailModel?> GetAsync(Guid id);

    public abstract Task<Guid> SaveAsync(TDetailModel model);

    public abstract Task DeleteAsync(Guid id);
}
