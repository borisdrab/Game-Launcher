using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class GameLibraryListPage : ContentPageBase
{
    public GameLibraryListPage(GameLibraryListViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
