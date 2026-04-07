using Launcher.BL.Models;

namespace Launcher.BL.Facades.Interfaces;

public interface IUserFacade : IFacade<UserListModel, UserDetailModel>
{
    Task<IEnumerable<UserListModel>> GetAsync(string? searchTerm, string? sortBy, bool ascending);
}