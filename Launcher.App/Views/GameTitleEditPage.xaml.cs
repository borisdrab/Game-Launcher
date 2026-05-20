using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class GameTitleEditPage : ContentPageBase
{
    public GameTitleEditPage(GameTitleEditViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}