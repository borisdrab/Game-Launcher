using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Launcher.App.Messages;
using Launcher.App.Services;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Models;

namespace Launcher.App.ViewModels;

[QueryProperty(nameof(UserIdString), "UserId")]
public partial class UserEditViewModel : ViewModelBase
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private static readonly Regex UserNameRegex =
        new(@"^[a-zA-Z0-9_]+$", RegexOptions.Compiled);

    private readonly IUserFacade _userFacade;
    private readonly INavigationService _navigationService;

    private Guid _userId;

    public string UserIdString
    {
        set
        {
            if (Guid.TryParse(value, out var id))
            {
                _userId = id;
                IsEditMode = id != Guid.Empty;
                ForceDataRefreshOnNextAppearing();
            }
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PageTitle))]
    private bool _isEditMode;

    public string PageTitle => IsEditMode ? "Edit User" : "Add User";

    [ObservableProperty]
    private IEnumerable<UserListModel> _users = [];

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _avatarUrl = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDisplayNameError))]
    private string _displayNameError = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUserNameError))]
    private string _userNameError = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasEmailError))]
    private string _emailError = string.Empty;

    public bool HasDisplayNameError => !string.IsNullOrEmpty(DisplayNameError);
    public bool HasUserNameError => !string.IsNullOrEmpty(UserNameError);
    public bool HasEmailError => !string.IsNullOrEmpty(EmailError);

    public UserEditViewModel(
        IUserFacade userFacade,
        INavigationService navigationService,
        IMessengerService messengerService)
        : base(messengerService)
    {
        _userFacade = userFacade;
        _navigationService = navigationService;
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        ClearErrors();
        await RefreshUsersAsync();

        if (IsEditMode && _userId != Guid.Empty)
        {
            var user = await _userFacade.GetAsync(_userId);
            if (user is not null)
            {
                DisplayName = user.DisplayName;
                UserName = user.UserName;
                Email = user.Email;
                AvatarUrl = user.AvatarUrl;
            }
        }
        else
        {
            ClearForm();
        }
    }

    private async Task RefreshUsersAsync()
    {
        Users = await _userFacade.GetAsync(null, "UserName", true);
    }

    private void ClearForm()
    {
        DisplayName = string.Empty;
        UserName = string.Empty;
        Email = string.Empty;
        AvatarUrl = string.Empty;
    }

    private void ClearErrors()
    {
        DisplayNameError = string.Empty;
        UserNameError = string.Empty;
        EmailError = string.Empty;
    }

    private bool Validate()
    {
        ClearErrors();
        bool valid = true;

        if (string.IsNullOrWhiteSpace(DisplayName))
        {
            DisplayNameError = "Name is required.";
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(UserName))
        {
            UserNameError = "Username is required.";
            valid = false;
        }
        else if (!UserNameRegex.IsMatch(UserName))
        {
            UserNameError = "Only letters, digits and underscore allowed.";
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            EmailError = "Email is required.";
            valid = false;
        }
        else if (!EmailRegex.IsMatch(Email))
        {
            EmailError = "Enter a valid email address.";
            valid = false;
        }

        return valid;
    }

    [RelayCommand]
    private async Task SelectUserAsync(Guid id)
    {
        _userId = id;
        IsEditMode = true;
        ClearErrors();

        var user = await _userFacade.GetAsync(id);
        if (user is not null)
        {
            DisplayName = user.DisplayName;
            UserName = user.UserName;
            Email = user.Email;
            AvatarUrl = user.AvatarUrl;
        }
    }

    [RelayCommand]
    private void AddNewUser()
    {
        _userId = Guid.Empty;
        IsEditMode = false;
        ClearForm();
        ClearErrors();
    }

    [RelayCommand]
    private async Task DeleteUserAsync(Guid id)
    {
        await _userFacade.DeleteAsync(id);

        if (id == _userId)
            AddNewUser();

        Messenger.Send(new UserChangedMessage());
        await RefreshUsersAsync();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!Validate()) return;

        var savedId = await _userFacade.SaveAsync(new UserDetailModel
        {
            Id = _userId,
            DisplayName = DisplayName.Trim(),
            UserName = UserName.Trim(),
            Email = Email.Trim(),
            AvatarUrl = AvatarUrl.Trim(),
        });

        _userId = savedId;
        IsEditMode = true;

        Messenger.Send(new UserChangedMessage());
        await RefreshUsersAsync();
    }

    [RelayCommand]
    private Task GoBackAsync()
        => _navigationService.GoToAsync("..");
}
