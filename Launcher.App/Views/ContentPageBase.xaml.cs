using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public abstract partial class ContentPageBase : ContentPage
{
    protected ViewModelBase ViewModel { get; }

    protected ContentPageBase(ViewModelBase viewModel)
    {
        InitializeComponent();

        BindingContext = ViewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await ViewModel.OnAppearingAsync();
    }
}
