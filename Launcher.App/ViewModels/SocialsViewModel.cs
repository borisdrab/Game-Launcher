using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Launcher.App.Messages;
using Launcher.App.Models;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;

namespace Launcher.App.ViewModels;

public partial class SocialsViewModel : ViewModelBase,
    IRecipient<ReviewEditMessage>,
    IRecipient<ReviewDeleteMessage>
{
    private readonly IReviewFacade _reviewFacade;
    private readonly IUserFacade _userFacade;
    private readonly IGameTitleFacade _gameTitleFacade;
    private readonly INavigationService _navigationService;
    private readonly IAlertService _alertService;
    private readonly ICurrentUserService _currentUserService;

    private List<ReviewDisplayItem> _allReviews = new();

    [ObservableProperty]
    private IEnumerable<ReviewDisplayItem> _reviews = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    public SocialsViewModel(
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
        await ReloadReviewsAsync();
    }

    private async Task ReloadReviewsAsync()
    {
        await _currentUserService.EnsureCurrentUserAsync();
        var currentUserId = _currentUserService.CurrentUser?.Id ?? Guid.Empty;

        var reviews = await _reviewFacade.GetAsync();
        var users = await _userFacade.GetAsync();
        var games = await _gameTitleFacade.GetAsync();

        var userNames = users.ToDictionary(u => u.Id, u => u.DisplayName);
        var gameNames = games.ToDictionary(g => g.Id, g => g.Name);

        _allReviews = reviews.Select(r => new ReviewDisplayItem
        {
            Id = r.Id,
            GameTitleId = r.GameTitleId,
            UserId = r.UserId,
            GameName = gameNames.TryGetValue(r.GameTitleId, out var gn) ? gn : "Unknown game",
            UserName = userNames.TryGetValue(r.UserId, out var un) ? un : "Unknown user",
            Rating = r.Rating,
            CreatedAt = r.CreatedAt,
            IsOwnedByCurrentUser = r.UserId == currentUserId
        }).ToList();

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Reviews = _allReviews;
        }
        else
        {
            Reviews = _allReviews
                .Where(r => r.GameName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                         || r.UserName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
    {
        await _navigationService.GoToAsync(
            NavigationService.ReviewDetailRouteRelative,
            new Dictionary<string, object?>
            {
                ["Id"] = id
            });
    }

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await _navigationService.GoToAsync(NavigationService.ReviewEditRouteRelative);
    }

    [RelayCommand]
    private async Task DeleteAsync(Guid id)
    {
        try
        {
            await _reviewFacade.DeleteAsync(id);
            MessengerService.Send(new ReviewDeleteMessage { ReviewId = id });
            await ReloadReviewsAsync();
        }
        catch (InvalidOperationException)
        {
            await _alertService.DisplayAsync("Delete error", "Review could not be deleted.");
        }
    }

    public void Receive(ReviewEditMessage message) => ForceDataRefreshOnNextAppearing();
    public void Receive(ReviewDeleteMessage message) => ForceDataRefreshOnNextAppearing();
}
