using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class ReviewEditPage : ContentPageBase
{
    public ReviewEditPage(ReviewEditViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
