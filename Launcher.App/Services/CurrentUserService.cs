using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Models;

namespace Launcher.App.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IUserFacade _userFacade;

    public UserListModel? CurrentUser { get; private set; }

    public CurrentUserService(IUserFacade userFacade)
    {
        _userFacade = userFacade;
    }

    public void SetCurrentUser(UserListModel user)
    {
        CurrentUser = user;
    }

    public async Task EnsureCurrentUserAsync()
    {
        // If no user is selected yet, pick the first one from the database as default
        if (CurrentUser is null)
        {
            var users = await _userFacade.GetAsync();
            CurrentUser = users.FirstOrDefault();
        }
    }
}
