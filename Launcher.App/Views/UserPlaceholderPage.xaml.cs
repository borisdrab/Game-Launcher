using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class UserPlaceholderPage : ContentPageBase
{
    public UserPlaceholderPage(UserPlaceholderViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
