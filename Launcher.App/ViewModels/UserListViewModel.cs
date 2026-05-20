using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Launcher.App.Messages;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Models;

namespace Launcher.App.ViewModels;

public partial class UserListViewModel : ViewModelBase
{
    private readonly IUserFacade _userFacade;
    private readonly INavigationService _navigationService;
    private readonly ICurrentUserService _currentUserService;

    [ObservableProperty]
    private IEnumerable<UserListModel> _users = [];

    public UserListViewModel(
        IUserFacade userFacade,
        INavigationService navigationService,
        ICurrentUserService currentUserService,
        IMessengerService messengerService)
        : base(messengerService)
    {
        _userFacade = userFacade;
        _navigationService = navigationService;
        _currentUserService = currentUserService;
    }

    protected override void OnActivated()
    {
        Messenger.Register<UserChangedMessage>(this, (_, _) =>
            MainThread.BeginInvokeOnMainThread(async () => await RefreshUsersAsync()));
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await RefreshUsersAsync();
    }

    private async Task RefreshUsersAsync()
    {
        Users = await _userFacade.GetAsync(null, "UserName", true);
    }

    [RelayCommand]
    private async Task SelectUserAsync(Guid id)
    {
        var picked = Users.FirstOrDefault(u => u.Id == id);
        if (picked is not null)
        {
            _currentUserService.SetCurrentUser(picked);
        }

        await _navigationService.GoToAsync(NavigationService.LibraryListRouteAbsolute);
    }

    [RelayCommand]
    private async Task AddUserAsync()
    {
        ForceDataRefreshOnNextAppearing();
        await _navigationService.GoToAsync(NavigationService.UserEditRoute);
    }

    [RelayCommand]
    private async Task EditUsersAsync()
    {
        ForceDataRefreshOnNextAppearing();
        await _navigationService.GoToAsync(NavigationService.UserEditRoute);
    }
}
