using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class GameTitleDetailPage : ContentPageBase
{
    public GameTitleDetailPage(GameTitleDetailViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}