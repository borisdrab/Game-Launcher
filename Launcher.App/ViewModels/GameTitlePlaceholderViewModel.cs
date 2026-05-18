using Launcher.App.Services;

namespace Launcher.App.ViewModels;

public class GameTitlePlaceholderViewModel : ViewModelBase
{
    public GameTitlePlaceholderViewModel(IMessengerService messengerService)
        : base(messengerService)
    {
    }
}
