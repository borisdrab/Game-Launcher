using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Repositories.Interfaces;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;

namespace Launcher.BL.Repositories;

public class PlatformRepository(LauncherDbContext ctx, IEntityMapper<PlatformEntity> entityMapper)
    : Repository<PlatformEntity>(ctx, entityMapper), IPlatformRepository
{
}
