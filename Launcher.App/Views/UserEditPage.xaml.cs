using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class UserEditPage : ContentPageBase
{
    public UserEditPage(UserEditViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
