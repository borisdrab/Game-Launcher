using Launcher.App.Models;
using Launcher.App.Views;

namespace Launcher.App.Services;

public class NavigationService : INavigationService
{
    // Route constants
    public const string GameListRouteAbsolute = "//games";
    public const string LibraryListRouteAbsolute = "//library";
    public const string SocialsRouteAbsolute = "//socials";
    public const string UserListRouteAbsolute = "//users";
    
    public const string GameDetailRouteRelative = "game-detail";
    public const string GameEditRouteRelative = "game-edit";

    public const string UserEditRoute = "useredit";

    public const string ReviewDetailRouteRelative = "review-detail";
    public const string ReviewEditRouteRelative = "review-edit";

    public IEnumerable<RouteModel> Routes { get; } = new List<RouteModel>
    {
        new(GameListRouteAbsolute, typeof(GameTitleListPage)),
        new(LibraryListRouteAbsolute, typeof(LibraryPlaceholderPage)),
        new(SocialsRouteAbsolute, typeof(SocialsPage)),
        new(UserListRouteAbsolute, typeof(UserListPage)),

        new(UserEditRoute, typeof(UserEditPage)),

        new(GameDetailRouteRelative, typeof(GameTitleDetailPage)),
        new(GameEditRouteRelative, typeof(GameTitleEditPage)),

        new(ReviewDetailRouteRelative, typeof(ReviewDetailPage)),
        new(ReviewEditRouteRelative, typeof(ReviewEditPage)),
    };

    public async Task GoToAsync(string route)
        => await Shell.Current.GoToAsync(route);

    public async Task GoToAsync(string route, IDictionary<string, object?> parameters)
        => await Shell.Current.GoToAsync(route, parameters);

    public bool SendBackButtonPressed()
        => Shell.Current.SendBackButtonPressed();
}
