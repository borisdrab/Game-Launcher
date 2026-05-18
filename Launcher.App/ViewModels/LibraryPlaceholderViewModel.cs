using Launcher.App.Services;

namespace Launcher.App.ViewModels;

public class LibraryPlaceholderViewModel : ViewModelBase
{
    public LibraryPlaceholderViewModel(IMessengerService messengerService)
        : base(messengerService)
    {
    }
}
