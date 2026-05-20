using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class GameLibraryDetailPage : ContentPageBase
{
    public GameLibraryDetailPage(GameLibraryDetailViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
