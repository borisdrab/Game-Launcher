using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using static Launcher.DAL.Seeds.UserSeeds;

namespace Launcher.DAL.Seeds;

public static class LibrarySeeds
{
    public static readonly LibraryEntity SchpagysCompleted = new()
    {
        Id = Guid.Parse("F31A1DAB-B3E7-446E-966C-A14FDC5AD007"),
        Name = "Completed",
        UserId = Jan.Id
    };
    
    public static readonly LibraryEntity BorissFavourite = new()
    {
        Id = Guid.Parse("4FD57535-3CE9-4B8D-9F1C-9978842013AD"),
        Name = "Favourite",
        UserId = Boris.Id
    };

    public static DbContext SeedLibraries(this DbContext dbx)
    {
        dbx.Set<LibraryEntity>().AddRange(
            new LibraryEntity { Id = SchpagysCompleted.Id, Name = SchpagysCompleted.Name, UserId = SchpagysCompleted.UserId },
            new LibraryEntity { Id = BorissFavourite.Id, Name = BorissFavourite.Name, UserId = BorissFavourite.UserId }
        );

        return dbx;
    }
}