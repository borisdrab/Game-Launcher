using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class GameTitleListPage : ContentPageBase
{
    public GameTitleListPage(GameTitleListViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
