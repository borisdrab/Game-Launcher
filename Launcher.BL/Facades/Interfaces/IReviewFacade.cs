using Launcher.BL.Models;

namespace Launcher.BL.Facades.Interfaces;

public interface IReviewFacade
    : IFacade<ReviewListModel, ReviewDetailModel>
{
    Task<IEnumerable<ReviewListModel>> GetAsync(QueryObject query);
}
