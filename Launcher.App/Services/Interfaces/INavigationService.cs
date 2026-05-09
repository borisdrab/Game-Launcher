using Launcher.App.Models;

namespace Launcher.App.Services;

public interface INavigationService
{
    IEnumerable<RouteModel> Routes { get; }

    Task GoToAsync(string route);
    Task GoToAsync(string route, IDictionary<string, object?> parameters);
    bool SendBackButtonPressed();
}
