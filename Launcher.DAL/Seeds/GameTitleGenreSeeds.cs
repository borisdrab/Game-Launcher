using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using static Launcher.DAL.Seeds.GenreSeeds;
using static Launcher.DAL.Seeds.GameTitleSeeds;

namespace Launcher.DAL.Seeds;

public static class GameTitleGenreSeeds
{
    public static readonly GameTitleGenreEntity TheWitcher3OpenWorld = new()
    {
        GameTitleId = TheWitcher3.Id,
        GenreId = OpenWorld.Id,
    };
    
    public static readonly GameTitleGenreEntity TheWitcher3Rpg = new()
    {
        GameTitleId = TheWitcher3.Id,
        GenreId = Rpg.Id,
    };

    public static readonly GameTitleGenreEntity EldenRingRpg = new()
    {
        GameTitleId = EldenRing.Id,
        GenreId = Rpg.Id,
    };

    public static DbContext SeedGameTitleGenres(this DbContext dbx)
    {
        dbx.Set<GameTitleGenreEntity>().AddRange(
            new GameTitleGenreEntity { GameTitleId = TheWitcher3OpenWorld.GameTitleId, GenreId = TheWitcher3OpenWorld.GenreId },
            new GameTitleGenreEntity { GameTitleId = TheWitcher3Rpg.GameTitleId, GenreId = TheWitcher3Rpg.GenreId },
            new GameTitleGenreEntity { GameTitleId = EldenRingRpg.GameTitleId, GenreId = EldenRingRpg.GenreId }
        );

        return dbx;
    }
}