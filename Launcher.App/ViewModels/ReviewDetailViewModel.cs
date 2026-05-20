using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Launcher.App.Messages;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Models;

namespace Launcher.App.ViewModels;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class ReviewDetailViewModel : ViewModelBase,
    IRecipient<ReviewEditMessage>
{
    private readonly IReviewFacade _reviewFacade;
    private readonly IUserFacade _userFacade;
    private readonly IGameTitleFacade _gameTitleFacade;
    private readonly INavigationService _navigationService;
    private readonly IAlertService _alertService;
    private readonly ICurrentUserService _currentUserService;

    public Guid Id { get; set; }

    [ObservableProperty]
    private ReviewDetailModel? _review;

    [ObservableProperty]
    private string _gameName = string.Empty;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private bool _canEdit;

    public ReviewDetailViewModel(
        IReviewFacade reviewFacade,
        IUserFacade userFacade,
        IGameTitleFacade gameTitleFacade,
        INavigationService navigationService,
        IAlertService alertService,
        ICurrentUserService currentUserService,
        IMessengerService messengerService)
        : base(messengerService)
    {
        _reviewFacade = reviewFacade;
        _userFacade = userFacade;
        _gameTitleFacade = gameTitleFacade;
        _navigationService = navigationService;
        _alertService = alertService;
        _currentUserService = currentUserService;
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        await _currentUserService.EnsureCurrentUserAsync();

        Review = await _reviewFacade.GetAsync(Id) ?? ReviewDetailModel.Empty;

        if (Review.GameTitleId != Guid.Empty)
        {
            var game = await _gameTitleFacade.GetAsync(Review.GameTitleId);
            GameName = game?.Name ?? "Unknown game";
        }

        if (Review.UserId != Guid.Empty)
        {
            var user = await _userFacade.GetAsync(Review.UserId);
            UserName = user?.DisplayName ?? "Unknown user";
        }

        CanEdit = _currentUserService.CurrentUser?.Id == Review.UserId;
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        if (Review is null || !CanEdit) return;

        await _navigationService.GoToAsync(
            NavigationService.ReviewEditRouteRelative,
            new Dictionary<string, object?>
            {
                ["Id"] = Review.Id
            });
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Review is null || !CanEdit) return;

        try
        {
            await _reviewFacade.DeleteAsync(Review.Id);
            MessengerService.Send(new ReviewDeleteMessage { ReviewId = Review.Id });
            _navigationService.SendBackButtonPressed();
        }
        catch (InvalidOperationException)
        {
            await _alertService.DisplayAsync("Delete error", "Review could not be deleted.");
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        _navigationService.SendBackButtonPressed();
        await Task.CompletedTask;
    }

    public void Receive(ReviewEditMessage message)
    {
        if (message.ReviewId == Review?.Id)
        {
            ForceDataRefreshOnNextAppearing();
        }
    }
}
