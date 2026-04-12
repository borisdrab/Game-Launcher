using Launcher.BL.Models;

namespace Launcher.BL.Facades.Interfaces;

public interface IAchievementFacade
    : IFacade<AchievementListModel, AchievementDetailModel>
{
    Task<IEnumerable<AchievementListModel>> GetAsync(QueryObject query);
}
