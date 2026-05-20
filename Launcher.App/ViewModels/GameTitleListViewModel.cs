using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Models;
using Launcher.BL.Repositories;

namespace Launcher.App.ViewModels;

public partial class GameTitleListViewModel : ViewModelBase
{
    private readonly IGameTitleFacade _gameTitleFacade;
    private readonly IGenreFacade _genreFacade;
    private readonly INavigationService _navigationService;
    private readonly IAlertService _alertService;

    [ObservableProperty] private IEnumerable<GameTitleListModel> _games = [];

    [ObservableProperty] private IEnumerable<GenreListModel> _genres = [];

    [ObservableProperty] private string _searchText = string.Empty;

    public GameTitleListViewModel(
        IGameTitleFacade gameTitleFacade,
        IGenreFacade genreFacade,
        INavigationService navigationService,
        IAlertService alertService,
        IMessengerService messengerService)
        : base(messengerService)
    {
        _gameTitleFacade = gameTitleFacade;
        _genreFacade = genreFacade;
        _navigationService = navigationService;
        _alertService = alertService;
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        await RunSafeAsync(async () =>
        {
            await LoadGamesAsync();
            Genres = await _genreFacade.GetAsync();
        }, _alertService);
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = HandleSearchAsync();
    }

    private async Task HandleSearchAsync()
    {
        await RunSafeAsync(async () =>
        {
            await LoadGamesAsync();
        }, _alertService);
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
    {
        // TODO: Navigate to game detail page when it is implemented
        await Task.CompletedTask;
    }

    private async Task LoadGamesAsync()
    {
        Games = await _gameTitleFacade.GetAsync(
            SearchText,
            null,
            null,
            null,
            GameTitleSortBy.Name,
            false);
    }
}