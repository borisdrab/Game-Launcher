using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using static Launcher.DAL.Seeds.GameTitleSeeds;
using static Launcher.DAL.Seeds.LibrarySeeds;

namespace Launcher.DAL.Seeds;

public static class LibraryTitleSeeds
{
    public static readonly LibraryTitleEntity SchpagysCompletedEldenRing = new()
    {
        LibraryId = SchpagysCompleted.Id,
        GameTitleId = EldenRing.Id,
        AddedAt = DateTime.Now,
        IsFavorite = true,
        PriceCentsAtPurchase = 5999
    };

    public static DbContext SeedLibraryTitles(this DbContext dbx)
    {
        dbx.Set<LibraryTitleEntity>().AddRange(
            new LibraryTitleEntity
            {
                LibraryId = SchpagysCompletedEldenRing.LibraryId,
                GameTitleId = SchpagysCompletedEldenRing.GameTitleId,
                AddedAt = SchpagysCompletedEldenRing.AddedAt,
                IsFavorite = SchpagysCompletedEldenRing.IsFavorite,
                PriceCentsAtPurchase = SchpagysCompletedEldenRing.PriceCentsAtPurchase
            }
        );

        return dbx;
    }
}