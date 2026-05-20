using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Launcher.App.Messages;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Facades;
using Launcher.BL.Models;

namespace Launcher.App.ViewModels;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class GameTitleEditViewModel(
    IGameTitleFacade gameTitleFacade,
    IAlertService alertService,
    INavigationService navigationService,
    IMessengerService messengerService)
    : ViewModelBase(messengerService)
{
    public Guid Id { get; set; }
    
    public bool CanDelete => Id != Guid.Empty;

    [ObservableProperty]
    private GameTitleDetailModel _gameTitle = GameTitleDetailModel.Empty;

    public string PageTitle => Id == Guid.Empty ? "Create Game Title" : "Edit Game Title";

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        if (Id == Guid.Empty)
        {
            GameTitle = GameTitleDetailModel.Empty;
            OnPropertyChanged(nameof(PageTitle));
            OnPropertyChanged(nameof(CanDelete));
            return;
        }

        GameTitle = await gameTitleFacade.GetAsync(Id)
                    ?? GameTitleDetailModel.Empty;

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
        navigationService.SendBackButtonPressed();
        await Task.CompletedTask;
    }
}