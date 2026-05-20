using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Launcher.App.Messages;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Models;
using Launcher.DAL.Entities;
using Launcher.DAL.Seeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Launcher.App.ViewModels;

[QueryProperty(nameof(GameTitleId), nameof(GameTitleId))]
public partial class GameLibraryDetailViewModel(
    IGameTitleFacade gameTitleFacade,
    ILibraryFacade libraryFacade,
    INavigationService navigationService,
    IMessengerService messengerService,
    IAlertService alertService,
    ICurrentUserService currentUserService)
    : ViewModelBase(messengerService),
      IRecipient<LibraryChangedMessage>
{

    public Guid GameTitleId { get; set; }

    [ObservableProperty]
    private GameTitleDetailModel? _gameTitle;

    [ObservableProperty]
    private LibraryTitleEntity? _libraryTitle;

    public string PriceInEurosText
        => LibraryTitle is null
            ? "0.00 €"
            : $"{LibraryTitle.PriceCentsAtPurchase / 100.0:F2} €";

    public string AverageRatingText
        => GameTitle?.AverageRating is null
            ? "0.0"
            : $"{GameTitle.AverageRating:F1}";

    public string FavoriteIcon
        => LibraryTitle?.IsFavorite == true ? "★" : "☆";

    public string FavoriteIconColor
        => LibraryTitle?.IsFavorite == true ? "#FFD700" : "#333333";

    partial void OnLibraryTitleChanged(LibraryTitleEntity? value)
    {
        OnPropertyChanged(nameof(PriceInEurosText));
        OnPropertyChanged(nameof(FavoriteIcon));
        OnPropertyChanged(nameof(FavoriteIconColor));
    }

    partial void OnGameTitleChanged(GameTitleDetailModel? value)
    {
        OnPropertyChanged(nameof(AverageRatingText));
    }



    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        GameTitle = await gameTitleFacade.GetAsync(GameTitleId)
                    ?? GameTitleDetailModel.Empty;

        GameTitle.ReleaseDate ??= DateTime.Today;

        await currentUserService.EnsureCurrentUserAsync();
        var userId = currentUserService.CurrentUser?.Id ?? Guid.Empty;

        var library = await libraryFacade.FilterAsync(userId, null, null, true);
        if (library != null && library.LibraryTitles != null)
        {
            LibraryTitle = library.LibraryTitles.FirstOrDefault(lt => lt.GameTitleId == GameTitleId);
        }
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        if (LibraryTitle != null)
        {
            await libraryFacade.ToggleFavoriteAsync(LibraryTitle.LibraryId, GameTitleId);
            await currentUserService.EnsureCurrentUserAsync();
            var userId = currentUserService.CurrentUser?.Id ?? Guid.Empty;
            
            var library = await libraryFacade.FilterAsync(userId, null, null, true);
            if (library != null && library.LibraryTitles != null)
            {
                LibraryTitle = library.LibraryTitles.FirstOrDefault(lt => lt.GameTitleId == GameTitleId);
            }
            
            MessengerService.Send(new LibraryChangedMessage());
        }
    }

    [RelayCommand]
    private async Task PlayAsync()
    {
        await alertService.DisplayAsync("Launcher", $"Starting game {GameTitle?.Name}... (Launcher functionality is disabled)");
    }

    [RelayCommand]
    private async Task RemoveAsync()
    {
        await currentUserService.EnsureCurrentUserAsync();
        var userId = currentUserService.CurrentUser?.Id ?? Guid.Empty;

        await libraryFacade.RemoveGameFromLibraryAsync(userId, GameTitleId);
        
        MessengerService.Send(new LibraryChangedMessage());
        navigationService.SendBackButtonPressed();
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        navigationService.SendBackButtonPressed();
        await Task.CompletedTask;
    }

    public void Receive(LibraryChangedMessage message)
    {
        ForceDataRefreshOnNextAppearing();
    }
}
