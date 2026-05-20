using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class ReviewDetailPage : ContentPageBase
{
    public ReviewDetailPage(ReviewDetailViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
