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
    ILibraryFacade libraryFacade,
    INavigationService navigationService,
    IMessengerService messengerService,
    IAlertService alertService)
    : ViewModelBase(messengerService),
      IRecipient<GameTitleEditMessage>,
      IRecipient<LibraryChangedMessage>
{
    public Guid Id { get; set; }

    [ObservableProperty]
    private GameTitleDetailModel? _gameTitle;

    [ObservableProperty]
    private bool _isInLibrary;

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

    public string PurchaseStatusText => IsInLibrary ? "In Library" : "Buy:";
    public string PurchasePriceText => IsInLibrary ? "Owned" : PriceInEurosText;
    public string PurchaseButtonColor => IsInLibrary ? "#888888" : "#32C852";

    public string FavoriteIcon => IsFavorite ? "★" : "☆";
    public string FavoriteIconColor => IsFavorite ? "#FFD700" : "#333333";

    partial void OnIsInLibraryChanged(bool value)
    {
        OnPropertyChanged(nameof(PurchaseStatusText));
        OnPropertyChanged(nameof(PurchasePriceText));
        OnPropertyChanged(nameof(PurchaseButtonColor));
    }

    partial void OnIsFavoriteChanged(bool value)
    {
        OnPropertyChanged(nameof(FavoriteIcon));
        OnPropertyChanged(nameof(FavoriteIconColor));
    }

    partial void OnGameTitleChanged(GameTitleDetailModel? value)
    {
        OnPropertyChanged(nameof(PriceInEurosText));
        OnPropertyChanged(nameof(AverageRatingText));
        OnPropertyChanged(nameof(PurchasePriceText));
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        GameTitle = await gameTitleFacade.GetAsync(Id)
                    ?? GameTitleDetailModel.Empty;

        GameTitle.ReleaseDate ??= DateTime.Today;

        IsInLibrary = await libraryFacade.IsGameInLibraryAsync(Launcher.DAL.Seeds.UserSeeds.Jan.Id, Id);

        var libraries = await libraryFacade.GetAsync();
        var userLibrary = libraries.FirstOrDefault(l => l.UserId == Launcher.DAL.Seeds.UserSeeds.Jan.Id);
        if (userLibrary != null)
        {
            var userLibraryDetail = await libraryFacade.GetAsync(userLibrary.Id);
            IsFavorite = userLibraryDetail?.LibraryTitles.FirstOrDefault(lt => lt.GameTitleId == Id)?.IsFavorite ?? false;
        }
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
        if (IsInLibrary) return;

        try
        {
            await libraryFacade.AddGameToLibraryAsync(Launcher.DAL.Seeds.UserSeeds.Jan.Id, Id);
            IsInLibrary = true;
            
            var libraries = await libraryFacade.GetAsync();
            var userLibrary = libraries.FirstOrDefault(l => l.UserId == Launcher.DAL.Seeds.UserSeeds.Jan.Id);
            if (userLibrary != null)
            {
                var userLibraryDetail = await libraryFacade.GetAsync(userLibrary.Id);
                IsFavorite = userLibraryDetail?.LibraryTitles.FirstOrDefault(lt => lt.GameTitleId == Id)?.IsFavorite ?? false;
            }

            MessengerService.Send(new LibraryChangedMessage());
            await alertService.DisplayAsync("Obchod", "Hra byla úspěšně zakoupena a přidána do Vaší knihovny!");
        }
        catch (Exception ex)
        {
            await alertService.DisplayAsync("Chyba nákupu", $"Nepodařilo se zakoupit hru: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        if (!IsInLibrary)
        {
            await alertService.DisplayAsync("Knihovna", "Tuto hru musíte nejprve vlastnit (koupit), abyste ji mohli přidat do oblíbených.");
            return;
        }

        var libraries = await libraryFacade.GetAsync();
        var userLibrary = libraries.FirstOrDefault(l => l.UserId == Launcher.DAL.Seeds.UserSeeds.Jan.Id);
        if (userLibrary != null)
        {
            await libraryFacade.ToggleFavoriteAsync(userLibrary.Id, Id);
            var userLibraryDetail = await libraryFacade.GetAsync(userLibrary.Id);
            IsFavorite = userLibraryDetail?.LibraryTitles.FirstOrDefault(lt => lt.GameTitleId == Id)?.IsFavorite ?? false;
            MessengerService.Send(new LibraryChangedMessage());
        }
    }

    public void Receive(GameTitleEditMessage message)
    {
        if (message.GameTitleId == GameTitle?.Id)
        {
            ForceDataRefreshOnNextAppearing();
        }
    }

    public void Receive(LibraryChangedMessage message)
    {
        ForceDataRefreshOnNextAppearing();
    }
}