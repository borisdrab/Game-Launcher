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
    private readonly INavigationService _navigationService;
    private readonly IAlertService _alertService;
    private readonly ICurrentUserService _currentUserService;

    // If Id is empty Guid -> we are creating a new review.
    // If Id has a value -> we are editing an existing review.
    public Guid Id { get; set; }

    [ObservableProperty]
    private ReviewDetailModel _review = new();

    // List of games the user can review
    // TODO: filter by user's library when library functionality is merged
    [ObservableProperty]
    private IEnumerable<GameTitleListModel> _games = [];

    // Currently selected game
    [ObservableProperty]
    private GameTitleListModel? _selectedGame;

    // The author - shown read-only, taken from CurrentUserService
    [ObservableProperty]
    private string _authorName = string.Empty;

    // Rating bound to slider AND label
    [ObservableProperty]
    private int _rating = 5;

    // Review text bound to editor
    [ObservableProperty]
    private string _reviewText = string.Empty;

    public string PageTitle => Id == Guid.Empty ? "New Review" : "Edit Review";

    public ReviewEditViewModel(
        IReviewFacade reviewFacade,
        IGameTitleFacade gameTitleFacade,
        INavigationService navigationService,
        IAlertService alertService,
        ICurrentUserService currentUserService,
        IMessengerService messengerService)
        : base(messengerService)
    {
        _reviewFacade = reviewFacade;
        _gameTitleFacade = gameTitleFacade;
        _navigationService = navigationService;
        _alertService = alertService;
        _currentUserService = currentUserService;
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        // Make sure we have a current user (defaults to first user in DB)
        await _currentUserService.EnsureCurrentUserAsync();
        AuthorName = _currentUserService.CurrentUser?.DisplayName ?? "(no user selected)";

        // Load games for the picker
        Games = await _gameTitleFacade.GetAsync();

        if (Id != Guid.Empty)
        {
            // Edit mode - load existing review
            Review = await _reviewFacade.GetAsync(Id) ?? new ReviewDetailModel();

            SelectedGame = Games.FirstOrDefault(g => g.Id == Review.GameTitleId);
            Rating = Review.Rating;
            ReviewText = Review.Text ?? string.Empty;
        }
        else
        {
            // Create mode - sensible defaults
            Review = new ReviewDetailModel();
            Rating = 5;
            ReviewText = string.Empty;
        }

        OnPropertyChanged(nameof(PageTitle));
    }

    // Clamp rating into 1-5 range
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

        // Fill in the review model
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
