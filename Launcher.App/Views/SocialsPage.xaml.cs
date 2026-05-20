using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class SocialsPage : ContentPageBase
{
    public SocialsPage(SocialsViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
