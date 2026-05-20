using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.App.Services;

namespace Launcher.App.ViewModels;

public abstract class ViewModelBase : ObservableRecipient
{
    private bool _forceDataRefresh = true;

    protected readonly IMessengerService MessengerService;

    protected ViewModelBase(IMessengerService messengerService)
        : base(messengerService.Messenger)
    {
        MessengerService = messengerService;

        IsActive = true;
    }

    public async Task OnAppearingAsync()
    {
        if (_forceDataRefresh)
        {
            await LoadDataAsync();

            _forceDataRefresh = false;
        }
    }

    protected void ForceDataRefreshOnNextAppearing()
    {
        _forceDataRefresh = true;
    }

    protected virtual Task LoadDataAsync()
        => Task.CompletedTask;

    protected async Task RunSafeAsync(
        Func<Task> action,
        IAlertService alertService,
        string errorTitle = "Error")
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            await alertService.DisplayAsync(errorTitle, ex.Message);
        }
    }
}
