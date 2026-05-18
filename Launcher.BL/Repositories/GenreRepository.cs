using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Repositories.Interfaces;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;

namespace Launcher.BL.Repositories;

public class GenreRepository(LauncherDbContext ctx, IEntityMapper<GenreEntity> entityMapper)
    : Repository<GenreEntity>(ctx, entityMapper), IGenreRepository
{
}
