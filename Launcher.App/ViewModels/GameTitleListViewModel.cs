using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Models;

namespace Launcher.App.ViewModels;

public partial class GameTitleListViewModel : ViewModelBase
{
    private readonly IGameTitleFacade _gameTitleFacade;
    private readonly IGenreFacade _genreFacade;
    private readonly INavigationService _navigationService;
    
    private IEnumerable<GameTitleListModel> _allGames = [];
    
    [ObservableProperty]
    private IEnumerable<GameTitleListModel> _games = [];
    
    [ObservableProperty]
    private IEnumerable<GenreListModel> _genres = [];
    
    [ObservableProperty]
    private string _searchText = string.Empty;

    public GameTitleListViewModel(
        IGameTitleFacade gameTitleFacade,
        IGenreFacade genreFacade,
        INavigationService navigationService,
        IMessengerService messengerService)
        : base(messengerService)
    {
        _gameTitleFacade = gameTitleFacade;
        _genreFacade = genreFacade;
        _navigationService = navigationService;
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        // Load all games and genres from database
        _allGames = await _gameTitleFacade.GetAsync();
        Games = _allGames;

        Genres = await _genreFacade.GetAsync();
    }
    
    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    // Filter games by search text
    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Games = _allGames;
        }
        else
        {
            Games = _allGames
                .Where(g => g.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
    {
        // TODO: Navigate to game detail page when it is implemented
        await Task.CompletedTask;
    }
}
