using Launcher.BL.Facades;
using Launcher.BL.Facades.Interfaces;
using Launcher.BL.Mappers;
using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Repositories;
using Launcher.BL.Repositories.Interfaces;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Launcher.BL;

public static class BLInstaller
{
    public static IServiceCollection AddBLServices(this IServiceCollection services)
    {
        // DbContext - created from factory each time
        services.AddTransient(sp =>
            sp.GetRequiredService<IDbContextFactory<LauncherDbContext>>().CreateDbContext());

        // Entity Mappers
        services.AddSingleton<IEntityMapper<GameTitleEntity>, GameTitleEntityMapper>();
        services.AddSingleton<IEntityMapper<UserEntity>, UserEntityMapper>();
        services.AddSingleton<IEntityMapper<LibraryEntity>, LibraryEntityMapper>();
        services.AddSingleton<IEntityMapper<GenreEntity>, GenreEntityMapper>();
        services.AddSingleton<IEntityMapper<PlatformEntity>, PlatformEntityMapper>();

        // Model Mappers
        services.AddSingleton<IModelMapper<GameTitleEntity, Models.GameTitleListModel, Models.GameTitleDetailModel>, GameTitleModelMapper>();
        services.AddSingleton<IModelMapper<UserEntity, Models.UserListModel, Models.UserDetailModel>, UserModelMapper>();
        services.AddSingleton<IModelMapper<LibraryEntity, Models.LibraryListModel, Models.LibraryDetailModel>, LibraryMapper>();
        services.AddSingleton<IModelMapper<GenreEntity, Models.GenreListModel, Models.GenreDetailModel>, GenreModelMapper>();
        services.AddSingleton<IModelMapper<PlatformEntity, Models.PlatformListModel, Models.PlatformDetailModel>, PlatformModelMapper>();
        services.AddSingleton<IModelMapper<AchievementEntity, Models.AchievementListModel, Models.AchievementDetailModel>, AchievementModelMapper>();
        services.AddSingleton<IModelMapper<ReviewEntity, Models.ReviewListModel, Models.ReviewDetailModel>, ReviewModelMapper>();
        services.AddSingleton<GameTitleModelMapper>();

        // Repositories
        services.AddTransient<IGameTitleRepository, GameTitleRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<IPlatformRepository, PlatformRepository>();
        services.AddTransient<LibraryRepository>();

        // Facades
        services.AddTransient<IGameTitleFacade, GameTitleFacade>();
        services.AddTransient<IUserFacade, UserFacade>();
        services.AddTransient<ILibraryFacade, LibraryFacade>();
        services.AddTransient<IGenreFacade, GenreFacade>();
        services.AddTransient<IPlatformFacade, PlatformFacade>();
        services.AddTransient<IAchievementFacade, AchievementFacade>();
        services.AddTransient<IReviewFacade, ReviewFacade>();

        return services;
    }
}
