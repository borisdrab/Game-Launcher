using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Launcher.App.Messages;
using Launcher.App.Models;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Launcher.App.ViewModels;

public enum LibrarySortBy
{
    Name,
    AddedAt
}

public partial class GameLibraryListViewModel : ViewModelBase,
    IRecipient<LibraryChangedMessage>
{
    private readonly ILibraryFacade _libraryFacade;
    private readonly IGenreFacade _genreFacade;
    private readonly INavigationService _navigationService;
    private readonly ICurrentUserService _currentUserService;

    [ObservableProperty]
    private IEnumerable<LibraryItemModel> _libraryItems = [];

    public ObservableCollection<GenreFilterItem> Genres { get; } = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private LibrarySortBy? _selectedSortBy;

    [ObservableProperty]
    private bool _sortDescending;

    public IEnumerable<LibrarySortBy> SortOptions { get; } = Enum.GetValues<LibrarySortBy>();

    public GameLibraryListViewModel(
        ILibraryFacade libraryFacade,
        IGenreFacade genreFacade,
        INavigationService navigationService,
        ICurrentUserService currentUserService,
        IMessengerService messengerService)
        : base(messengerService)
    {
        _libraryFacade = libraryFacade;
        _genreFacade = genreFacade;
        _navigationService = navigationService;
        _currentUserService = currentUserService;

        Genres.CollectionChanged += Genres_CollectionChanged;
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        await _currentUserService.EnsureCurrentUserAsync();

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
            NavigationService.LibraryDetailRouteRelative,
            new Dictionary<string, object?>
            {
                ["GameTitleId"] = id
            });
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
        var selectedGenreNames = Genres
            .Where(g => g.IsSelected)
            .Select(g => g.Name)
            .ToArray();

        string? sortBy = SelectedSortBy switch
        {
            LibrarySortBy.Name => "name",
            LibrarySortBy.AddedAt => "addedat",
            _ => null
        };

        await _currentUserService.EnsureCurrentUserAsync();
        var userId = _currentUserService.CurrentUser?.Id ?? Guid.Empty;

        var library = await _libraryFacade.FilterAsync(
            userId,
            string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
            sortBy,
            !SortDescending,
            selectedGenreNames);

        if (library != null && library.LibraryTitles != null)
        {
            LibraryItems = library.LibraryTitles
                .Select(lt => new LibraryItemModel
                {
                    Id = lt.GameTitleId,
                    Name = lt.GameTitle?.Name ?? string.Empty,
                    CoverImageUrl = lt.GameTitle?.CoverImageUrl ?? string.Empty,
                    Publisher = lt.GameTitle?.Publisher ?? string.Empty,
                    PegiRating = lt.GameTitle?.PegiRating ?? 0,
                    ReleaseDate = lt.GameTitle?.ReleaseDate,
                    PriceCentsAtPurchase = lt.PriceCentsAtPurchase,
                    IsFavorite = lt.IsFavorite
                })
                .ToList();
        }
        else
        {
            LibraryItems = Array.Empty<LibraryItemModel>();
        }
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync(Guid gameTitleId)
    {
        await _currentUserService.EnsureCurrentUserAsync();
        var userId = _currentUserService.CurrentUser?.Id ?? Guid.Empty;

        var libraries = await _libraryFacade.GetAsync();
        var userLibrary = libraries.FirstOrDefault(l => l.UserId == userId);
        if (userLibrary != null)
        {
            await _libraryFacade.ToggleFavoriteAsync(userLibrary.Id, gameTitleId);
            await ReloadGamesAsync();
            MessengerService.Send(new LibraryChangedMessage());
        }
    }

    partial void OnSelectedSortByChanged(LibrarySortBy? value)
    {
        SearchCommand.Execute(null);
    }

    partial void OnSortDescendingChanged(bool value)
    {
        SearchCommand.Execute(null);
    }

    public void Receive(LibraryChangedMessage message)
    {
        ForceDataRefreshOnNextAppearing();
        _ = ReloadGamesAsync();
    }
}
