using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Launcher.App.Messages;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Models;

namespace Launcher.App.ViewModels;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class ReviewEditViewModel : ViewModelBase
{
    private readonly IReviewFacade _reviewFacade;
    private readonly IGameTitleFacade _gameTitleFacade;
    private readonly ILibraryFacade _libraryFacade;
    private readonly INavigationService _navigationService;
    private readonly IAlertService _alertService;
    private readonly ICurrentUserService _currentUserService;

    public Guid Id { get; set; }

    [ObservableProperty]
    private ReviewDetailModel _review = new();

    [ObservableProperty]
    private IEnumerable<GameTitleListModel> _games = [];

    [ObservableProperty]
    private GameTitleListModel? _selectedGame;

    [ObservableProperty]
    private bool _hasNoGames;

    [ObservableProperty]
    private string _authorName = string.Empty;

    [ObservableProperty]
    private int _rating = 5;

    [ObservableProperty]
    private string _reviewText = string.Empty;

    public string PageTitle => Id == Guid.Empty ? "New Review" : "Edit Review";

    public ReviewEditViewModel(
        IReviewFacade reviewFacade,
        IGameTitleFacade gameTitleFacade,
        ILibraryFacade libraryFacade,
        INavigationService navigationService,
        IAlertService alertService,
        ICurrentUserService currentUserService,
        IMessengerService messengerService)
        : base(messengerService)
    {
        _reviewFacade = reviewFacade;
        _gameTitleFacade = gameTitleFacade;
        _libraryFacade = libraryFacade;
        _navigationService = navigationService;
        _alertService = alertService;
        _currentUserService = currentUserService;
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        await _currentUserService.EnsureCurrentUserAsync();
        AuthorName = _currentUserService.CurrentUser?.DisplayName ?? "(no user selected)";

        var userId = _currentUserService.CurrentUser?.Id ?? Guid.Empty;

        if (Id != Guid.Empty)
        {
            Review = await _reviewFacade.GetAsync(Id) ?? new ReviewDetailModel();
            Games = await _gameTitleFacade.GetAsync();

            SelectedGame = Games.FirstOrDefault(g => g.Id == Review.GameTitleId);
            Rating = Review.Rating;
            ReviewText = Review.Text ?? string.Empty;
        }
        else
        {
            var library = await _libraryFacade.FilterAsync(userId, null, null, true);
            var libraryGameIds = library?.LibraryTitles?
                .Select(lt => lt.GameTitleId)
                .ToHashSet() ?? new HashSet<Guid>();

            var allGames = await _gameTitleFacade.GetAsync();
            Games = allGames.Where(g => libraryGameIds.Contains(g.Id)).ToList();

            Review = new ReviewDetailModel();
            Rating = 5;
            ReviewText = string.Empty;
        }

        HasNoGames = !Games.Any();
        OnPropertyChanged(nameof(PageTitle));
    }

    partial void OnRatingChanged(int value)
    {
        if (value < 1) Rating = 1;
        else if (value > 5) Rating = 5;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (_currentUserService.CurrentUser is null)
        {
            await _alertService.DisplayAsync("No user", "No user is selected. Pick a user in the Users tab first.");
            return;
        }

        if (SelectedGame is null)
        {
            await _alertService.DisplayAsync("Validation", "Please select a game.");
            return;
        }

        if (Rating < 1 || Rating > 5)
        {
            await _alertService.DisplayAsync("Validation", "Rating must be between 1 and 5.");
            return;
        }

        Review.GameTitleId = SelectedGame.Id;
        Review.UserId = _currentUserService.CurrentUser.Id;
        Review.Rating = Rating;
        Review.Text = ReviewText;

        if (Id == Guid.Empty)
        {
            Review.CreatedAt = DateTime.Now;
        }
        else
        {
            Review.UpdatedAt = DateTime.Now;
        }

        try
        {
            var savedId = await _reviewFacade.SaveAsync(Review);
            MessengerService.Send(new ReviewEditMessage { ReviewId = savedId });
            _navigationService.SendBackButtonPressed();
        }
        catch (Exception)
        {
            await _alertService.DisplayAsync("Save error", "Could not save the review.");
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        _navigationService.SendBackButtonPressed();
        await Task.CompletedTask;
    }
}
