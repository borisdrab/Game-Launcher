using Launcher.BL.Models;

namespace Launcher.BL.Facades.Interfaces;

public interface IPlatformFacade
    : IFacade<PlatformListModel, PlatformDetailModel>
{
    Task<IEnumerable<PlatformListModel>> GetAsync(QueryObject query);
}
