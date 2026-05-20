using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Launcher.App.Messages;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Models;

namespace Launcher.App.ViewModels;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class GameTitleDetailViewModel(
    IGameTitleFacade gameTitleFacade,
    INavigationService navigationService,
    IMessengerService messengerService,
    IAlertService alertService)
    : ViewModelBase(messengerService), IRecipient<GameTitleEditMessage>
{
    public Guid Id { get; set; }

    [ObservableProperty]
    private GameTitleDetailModel? _gameTitle;
    
    [ObservableProperty]
    private bool _isFavorite;
    
    public string PriceInEurosText
        => GameTitle is null
            ? "0.00 €"
            : $"{GameTitle.PriceCents / 100.0:F2} €";
    
    public string AverageRatingText
        => GameTitle?.AverageRating is null
            ? "0.0"
            : $"{GameTitle.AverageRating:F1}";

    partial void OnGameTitleChanged(GameTitleDetailModel? value)
    {
        OnPropertyChanged(nameof(PriceInEurosText));
        OnPropertyChanged(nameof(AverageRatingText));
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        GameTitle = await gameTitleFacade.GetAsync(Id)
                    ?? GameTitleDetailModel.Empty;

        GameTitle.ReleaseDate ??= DateTime.Today;
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (GameTitle is not null)
        {
            try
            {
                await gameTitleFacade.DeleteAsync(GameTitle.Id);

                MessengerService.Send(new GameTitleDeleteMessage());

                navigationService.SendBackButtonPressed();
            }
            catch (InvalidOperationException)
            {
                await alertService.DisplayAsync("Delete error", "Game title could not be deleted.");
            }
        }
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        if (GameTitle is not null)
        {
            await navigationService.GoToAsync(
                NavigationService.GameEditRouteRelative,
                new Dictionary<string, object?>
                {
                    [nameof(GameTitleEditViewModel.Id)] = GameTitle.Id
                });
        }
    }
    
    [RelayCommand]
    private async Task GoBackAsync()
    {
        navigationService.SendBackButtonPressed();
        await Task.CompletedTask;
    }
    
    [RelayCommand]
    private async Task BuyAsync()
    {
        if (GameTitle is null)
        {
            return;
        }

        await alertService.DisplayAsync("Buy", $"Buying {GameTitle.Name}.");
    }
    
    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        IsFavorite = !IsFavorite;
        await Task.CompletedTask;
    }

    public void Receive(GameTitleEditMessage message)
    {
        if (message.GameTitleId == GameTitle?.Id)
        {
            ForceDataRefreshOnNextAppearing();
        }
    }
}