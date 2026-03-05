using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.DAL.Seeds;

public static class PlatformSeeds
{
    public static readonly PlatformEntity Pc = new()
    {
        Id = Guid.Parse("3A5F07F2-2D89-40E7-A16B-03BD3A69E3C1"),
        Name = "PC"
    };

    public static readonly PlatformEntity Xbox = new()
    {
        Id = Guid.Parse("EB2418D7-B576-4015-8917-0B2DCD56AAF8"),
        Name = "Xbox"
    };

    public static readonly PlatformEntity PlayStation5 = new()
    {
        Id = Guid.Parse("A7CD98E1-F9C0-45F3-96FC-BBC6BF80B41D"),
        Name = "PlayStation 5"
    };

    public static DbContext SeedPlatforms(this DbContext dbx)
    {
        dbx.Set<PlatformEntity>().AddRange(
            Pc,
            Xbox,
            PlayStation5
        );

        return dbx;
    }
}