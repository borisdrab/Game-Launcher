using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Launcher.App.Messages;
using Launcher.App.Models;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Models;
using System.Collections.ObjectModel;

namespace Launcher.App.ViewModels;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class GameTitleEditViewModel(
    IGameTitleFacade gameTitleFacade,
    IGenreFacade genreFacade,
    IPlatformFacade platformFacade,
    INavigationService navigationService,
    IMessengerService messengerService,
    IAlertService alertService)
    : ViewModelBase(messengerService)
{
    public Guid Id { get; set; }

    [ObservableProperty]
    private GameTitleDetailModel _gameTitle = GameTitleDetailModel.Empty;

    public ObservableCollection<SelectableItem> AvailableGenres { get; } = [];
    public ObservableCollection<SelectableItem> AvailablePlatforms { get; } = [];

    public string PageTitle => Id == Guid.Empty ? "Create Game Title" : "Edit Game Title";
    public bool CanDelete => Id != Guid.Empty;

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        AvailableGenres.Clear();
        AvailablePlatforms.Clear();

        var allGenres = await genreFacade.GetAsync();
        var allPlatforms = await platformFacade.GetAsync();

        if (Id == Guid.Empty)
        {
            GameTitle = new GameTitleDetailModel
            {
                IsAvailable = true
            };

            foreach (var genre in allGenres)
            {
                AvailableGenres.Add(new SelectableItem
                {
                    Id = genre.Id,
                    Name = genre.Name,
                    IsSelected = false
                });
            }

            foreach (var platform in allPlatforms)
            {
                AvailablePlatforms.Add(new SelectableItem
                {
                    Id = platform.Id,
                    Name = platform.Name,
                    IsSelected = false
                });
            }

            OnPropertyChanged(nameof(PageTitle));
            OnPropertyChanged(nameof(CanDelete));
            return;
        }

        GameTitle = await gameTitleFacade.GetAsync(Id)
                    ?? GameTitleDetailModel.Empty;

        var selectedGenreIds = GameTitle.Genres.Select(x => x.Id).ToHashSet();
        var selectedPlatformIds = GameTitle.Platforms.Select(x => x.Id).ToHashSet();

        foreach (var genre in allGenres)
        {
            AvailableGenres.Add(new SelectableItem
            {
                Id = genre.Id,
                Name = genre.Name,
                IsSelected = selectedGenreIds.Contains(genre.Id)
            });
        }

        foreach (var platform in allPlatforms)
        {
            AvailablePlatforms.Add(new SelectableItem
            {
                Id = platform.Id,
                Name = platform.Name,
                IsSelected = selectedPlatformIds.Contains(platform.Id)
            });
        }

        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(CanDelete));
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(GameTitle.Name))
        {
            await alertService.DisplayAsync("Validation error", "Name is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(GameTitle.Description))
        {
            await alertService.DisplayAsync("Validation error", "Description is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(GameTitle.Publisher))
        {
            await alertService.DisplayAsync("Validation error", "Publisher is required.");
            return;
        }

        if (GameTitle.PegiRating < 0 || GameTitle.PegiRating > 18)
        {
            await alertService.DisplayAsync("Validation error", "PEGI rating must be between 0 and 18.");
            return;
        }

        if (GameTitle.PriceCents < 0)
        {
            await alertService.DisplayAsync("Validation error", "Price must not be negative.");
            return;
        }

        try
        {
            var modelToSave = new GameTitleDetailModel
            {
                Id = GameTitle.Id,
                Name = GameTitle.Name,
                Description = GameTitle.Description,
                PegiRating = GameTitle.PegiRating,
                PriceCents = GameTitle.PriceCents,
                CoverImageUrl = GameTitle.CoverImageUrl,
                Publisher = GameTitle.Publisher,
                ReleaseDate = GameTitle.ReleaseDate,
                IsAvailable = GameTitle.IsAvailable,
                Genres = [],
                Platforms = [],
                Achievements = [],
                Reviews = []
            };

            var savedId = await gameTitleFacade.SaveAsync(modelToSave);

            await SyncGenresAsync(savedId);
            await SyncPlatformsAsync(savedId);

            MessengerService.Send(new GameTitleEditMessage
            {
                GameTitleId = savedId
            });

            await navigationService.GoToAsync(NavigationService.GameListRouteAbsolute);
        }
        catch (Exception ex)
        {
            await alertService.DisplayAsync("Save error", ex.Message);
        }
    }

    private async Task SyncGenresAsync(Guid gameTitleId)
    {
        var currentDetail = await gameTitleFacade.GetAsync(gameTitleId);
        if (currentDetail is null)
        {
            throw new InvalidOperationException("Saved game title could not be loaded.");
        }

        var currentGenreIds = currentDetail.Genres.Select(x => x.Id).ToHashSet();
        var selectedGenreIds = AvailableGenres.Where(x => x.IsSelected).Select(x => x.Id).ToHashSet();

        foreach (var genreId in selectedGenreIds.Except(currentGenreIds))
        {
            await gameTitleFacade.AddGenreAsync(gameTitleId, genreId);
        }

        foreach (var genreId in currentGenreIds.Except(selectedGenreIds))
        {
            await gameTitleFacade.RemoveGenreAsync(gameTitleId, genreId);
        }
    }

    private async Task SyncPlatformsAsync(Guid gameTitleId)
    {
        var currentDetail = await gameTitleFacade.GetAsync(gameTitleId);
        if (currentDetail is null)
        {
            throw new InvalidOperationException("Saved game title could not be loaded.");
        }

        var currentPlatformIds = currentDetail.Platforms.Select(x => x.Id).ToHashSet();
        var selectedPlatformIds = AvailablePlatforms.Where(x => x.IsSelected).Select(x => x.Id).ToHashSet();

        foreach (var platformId in selectedPlatformIds.Except(currentPlatformIds))
        {
            await gameTitleFacade.AddPlatformAsync(gameTitleId, platformId);
        }

        foreach (var platformId in currentPlatformIds.Except(selectedPlatformIds))
        {
            await gameTitleFacade.RemovePlatformAsync(gameTitleId, platformId);
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Id == Guid.Empty)
        {
            return;
        }

        try
        {
            await gameTitleFacade.DeleteAsync(Id);

            MessengerService.Send(new GameTitleDeleteMessage());

            await navigationService.GoToAsync(NavigationService.GameListRouteAbsolute);
        }
        catch (Exception ex)
        {
            await alertService.DisplayAsync("Delete error", ex.Message);
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await navigationService.GoToAsync(NavigationService.GameListRouteAbsolute);
    }
}