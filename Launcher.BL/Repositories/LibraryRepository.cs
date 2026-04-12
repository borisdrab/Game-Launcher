using Launcher.BL.Mappers.Interfaces;
using Launcher.BL.Repositories.Interfaces;
using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Launcher.BL.Repositories
{
    public class LibraryRepository(LauncherDbContext ctx, IEntityMapper<LibraryEntity> entityMapper) : Repository<LibraryEntity>(ctx, entityMapper)
    {

    }
}
