using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Launcher.App.Messages;
using Launcher.App.Models;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Models;
using Launcher.BL.Repositories;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Launcher.App.ViewModels;

public partial class GameTitleListViewModel : ViewModelBase,
    IRecipient<GameTitleEditMessage>,
    IRecipient<GameTitleDeleteMessage>
{
    private readonly IGameTitleFacade _gameTitleFacade;
    private readonly IGenreFacade _genreFacade;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private IEnumerable<GameTitleListModel> _games = [];

    public ObservableCollection<GenreFilterItem> Genres { get; } = [];

    [ObservableProperty]
    private string _searchText = string.Empty;
    
    [ObservableProperty]
    private GameTitleSortBy? _selectedSortBy;

    [ObservableProperty]
    private bool _sortDescending;

    public IEnumerable<GameTitleSortBy> SortOptions { get; } = Enum.GetValues<GameTitleSortBy>();

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

        Genres.CollectionChanged += Genres_CollectionChanged;
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        if (Genres.Count == 0)
        {
            var genreModels = await _genreFacade.GetAsync();

            foreach (var genre in genreModels)
            {
                var item = GenreFilterItem.FromModel(genre);
                item.PropertyChanged += GenreItem_PropertyChanged;
                Genres.Add(item);
            }
        }

        await ReloadGamesAsync();
    }

    private void Genres_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (GenreFilterItem item in e.NewItems)
            {
                item.PropertyChanged += GenreItem_PropertyChanged;
            }
        }

        if (e.OldItems is not null)
        {
            foreach (GenreFilterItem item in e.OldItems)
            {
                item.PropertyChanged -= GenreItem_PropertyChanged;
            }
        }
    }

    private async void GenreItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GenreFilterItem.IsSelected))
        {
            await ReloadGamesAsync();
        }
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
    {
        await _navigationService.GoToAsync(
            NavigationService.GameDetailRouteRelative,
            new Dictionary<string, object?>
            {
                ["Id"] = id
            });
    }

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await _navigationService.GoToAsync(NavigationService.GameEditRouteRelative);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await ReloadGamesAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        SearchCommand.Execute(null);
    }

    private async Task ReloadGamesAsync()
    {
        var selectedGenreIds = Genres
            .Where(g => g.IsSelected)
            .Select(g => g.Id)
            .ToList();

        Games = await _gameTitleFacade.GetAsync(
            string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
            null,
            true,
            null,
            selectedGenreIds,
            SelectedSortBy,
            SortDescending);
    }
    
    [RelayCommand]
    private async Task ToggleFavoriteAsync(Guid id)
    {
        await Task.CompletedTask;
    }
    
    partial void OnSelectedSortByChanged(GameTitleSortBy? value)
    {
        SearchCommand.Execute(null);
    }

    partial void OnSortDescendingChanged(bool value)
    {
        SearchCommand.Execute(null);
    }

    public void Receive(GameTitleEditMessage message)
    {
        ForceDataRefreshOnNextAppearing();
    }

    public void Receive(GameTitleDeleteMessage message)
    {
        ForceDataRefreshOnNextAppearing();
    }
}