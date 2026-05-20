using Launcher.BL.Models;

namespace Launcher.App.Services;

public interface ICurrentUserService
{
    UserListModel? CurrentUser { get; }

    void SetCurrentUser(UserListModel user);

    Task EnsureCurrentUserAsync();
}
