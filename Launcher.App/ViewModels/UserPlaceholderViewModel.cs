using Launcher.App.Services;

namespace Launcher.App.ViewModels;

public class UserPlaceholderViewModel : ViewModelBase
{
    public UserPlaceholderViewModel(IMessengerService messengerService)
        : base(messengerService)
    {
    }
}
