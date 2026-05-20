using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class UserListPage : ContentPageBase
{
    public UserListPage(UserListViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
