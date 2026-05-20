using Launcher.BL.Models;

namespace Launcher.App.Services;

// Holds the currently selected user (the one writing reviews, etc.).
// Acts like a simple "logged in user" but without real authentication.
public interface ICurrentUserService
{
    UserListModel? CurrentUser { get; }

    // Called when the user picks a profile in the Users tab.
    void SetCurrentUser(UserListModel user);

    // Ensures we have a current user (sets the first user from DB if none).
    Task EnsureCurrentUserAsync();
}
