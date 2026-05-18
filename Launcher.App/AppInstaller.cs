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

        // Views
        services.AddTransient<GameTitleListPage>();
        services.AddTransient<LibraryPlaceholderPage>();
        services.AddTransient<SocialsPlaceholderPage>();
        services.AddTransient<UserPlaceholderPage>();

        // ViewModels
        services.AddTransient<GameTitleListViewModel>();
        services.AddTransient<LibraryPlaceholderViewModel>();
        services.AddTransient<SocialsPlaceholderViewModel>();
        services.AddTransient<UserPlaceholderViewModel>();

        return services;
    }
}
