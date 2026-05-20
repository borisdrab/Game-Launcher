using CommunityToolkit.Mvvm.Messaging;
using Launcher.App.Services;
using Launcher.App.Shells;
using Launcher.App.ViewModels;
using Launcher.App.Views;

namespace Launcher.App;

public static class AppInstaller
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<AppShell>();

        services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);

        services.AddSingleton<IMessengerService, MessengerService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IAlertService, AlertService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        // Views
        services.AddTransient<GameTitleListPage>();
        services.AddTransient<GameTitleDetailPage>();
        services.AddTransient<GameTitleEditPage>();
        services.AddTransient<GameLibraryListPage>();
        services.AddTransient<GameLibraryDetailPage>();
        services.AddTransient<SocialsPage>();
        services.AddTransient<ReviewDetailPage>();
        services.AddTransient<ReviewEditPage>();
        services.AddTransient<UserListPage>();
        services.AddTransient<UserEditPage>();

        // ViewModels
        services.AddTransient<GameTitleListViewModel>();
        services.AddTransient<GameTitleDetailViewModel>();
        services.AddTransient<GameTitleEditViewModel>();
        services.AddTransient<GameLibraryListViewModel>();
        services.AddTransient<GameLibraryDetailViewModel>();
        services.AddTransient<SocialsViewModel>();
        services.AddTransient<ReviewDetailViewModel>();
        services.AddTransient<ReviewEditViewModel>();
        services.AddTransient<UserListViewModel>();
        services.AddTransient<UserEditViewModel>();

        return services;
    }
}
