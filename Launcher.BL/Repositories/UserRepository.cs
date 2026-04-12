using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Repositories.Interfaces;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;

namespace Launcher.BL.Repositories;

public class UserRepository(LauncherDbContext ctx, IEntityMapper<UserEntity> entityMapper)
    : Repository<UserEntity>(ctx, entityMapper), IUserRepository
{
}