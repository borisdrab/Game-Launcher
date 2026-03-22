using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Models;
using Launcher.DAL.Entities;

namespace Launcher.BL.Facades;

public class GameTitleFacade
    : FacadeBase<GameTitleEntity, GameTitleListModel, GameTitleDetailModel>,
        IGameTitleFacade
{
    public GameTitleFacade(
        IModelMapper<GameTitleEntity, GameTitleListModel, GameTitleDetailModel> mapper)
        : base(mapper)
    {
    }

    public override Task<IEnumerable<GameTitleListModel>> GetAsync()
        => throw new NotImplementedException();

    public override Task<GameTitleDetailModel?> GetAsync(Guid id)
        => throw new NotImplementedException();

    public override Task<Guid> SaveAsync(GameTitleDetailModel model)
        => throw new NotImplementedException();

    public override Task DeleteAsync(Guid id)
        => throw new NotImplementedException();
}