using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Launcher.App.Messages;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Models;
using Launcher.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Launcher.App.ViewModels;

[QueryProperty(nameof(GameTitleId), nameof(GameTitleId))]
public partial class GameLibraryDetailViewModel : ViewModelBase,
    IRecipient<LibraryChangedMessage>
{
    private readonly IGameTitleFacade _gameTitleFacade;
    private readonly ILibraryFacade _libraryFacade;
    private readonly INavigationService _navigationService;
    private readonly IAlertService _alertService;
    private readonly ICurrentUserService _currentUserService;

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

    public GameLibraryDetailViewModel(
        IGameTitleFacade gameTitleFacade,
        ILibraryFacade libraryFacade,
        INavigationService navigationService,
        ICurrentUserService currentUserService,
        IMessengerService messengerService,
        IAlertService alertService)
        : base(messengerService)
    {
        _gameTitleFacade = gameTitleFacade;
        _libraryFacade = libraryFacade;
        _navigationService = navigationService;
        _currentUserService = currentUserService;
        _alertService = alertService;
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        await _currentUserService.EnsureCurrentUserAsync();
        var userId = _currentUserService.CurrentUser?.Id ?? Guid.Empty;

        GameTitle = await _gameTitleFacade.GetAsync(GameTitleId)
                    ?? GameTitleDetailModel.Empty;

        GameTitle.ReleaseDate ??= DateTime.Today;

        var library = await _libraryFacade.FilterAsync(userId, null, null, true);
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
            await _libraryFacade.ToggleFavoriteAsync(LibraryTitle.LibraryId, GameTitleId);
            
            var userId = _currentUserService.CurrentUser?.Id ?? Guid.Empty;
            var library = await _libraryFacade.FilterAsync(userId, null, null, true);
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
        await _alertService.DisplayAsync("Launcher", $"Starting game {GameTitle?.Name}... (Launcher functionality is disabled)");
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        _navigationService.SendBackButtonPressed();
        await Task.CompletedTask;
    }

    public void Receive(LibraryChangedMessage message)
    {
        ForceDataRefreshOnNextAppearing();
    }
}
