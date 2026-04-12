using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using static Launcher.DAL.Seeds.GameTitleSeeds;
using static Launcher.DAL.Seeds.PlatformSeeds;

namespace Launcher.DAL.Seeds;

public static class GameTitlePlatformSeeds
{
    public static readonly GameTitlePlatformEntity EldenRingPc = new()
    {
        GameTitleId = EldenRing.Id,
        PlatformId = Pc.Id
    };

    public static readonly GameTitlePlatformEntity EldenRingXbox = new()
    {
        GameTitleId = EldenRing.Id,
        PlatformId = Xbox.Id
    };

    public static DbContext SeedGameTitlePlatforms(this DbContext dbx)
    {
        dbx.Set<GameTitlePlatformEntity>().AddRange(
            new GameTitlePlatformEntity { GameTitleId = EldenRingPc.GameTitleId, PlatformId = EldenRingPc.PlatformId },
            new GameTitlePlatformEntity { GameTitleId = EldenRingXbox.GameTitleId, PlatformId = EldenRingXbox.PlatformId }
        );

        return dbx;
    }
}