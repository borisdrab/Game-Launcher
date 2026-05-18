using CommunityToolkit.Mvvm.Input;
using Launcher.App.Services;

namespace Launcher.App.Shells;

public partial class AppShell : Shell
{
    private readonly INavigationService _navigationService;

    public AppShell(INavigationService navigationService)
    {
        _navigationService = navigationService;

        InitializeComponent();
    }
    
    [RelayCommand]
    private async Task GoToGamesAsync()
        => await _navigationService.GoToAsync(NavigationService.GameListRouteAbsolute);

    [RelayCommand]
    private async Task GoToUsersAsync()
        => await _navigationService.GoToAsync(NavigationService.UserListRouteAbsolute);

    [RelayCommand]
    private async Task GoToLibraryAsync()
        => await _navigationService.GoToAsync(NavigationService.LibraryListRouteAbsolute);
}
