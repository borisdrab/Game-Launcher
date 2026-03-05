using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.DAL.Seeds;

public static class GenreSeeds
{
    public static readonly GenreEntity ActionRpg = new()
    {
        Id = Guid.Parse("F74281DB-B42B-4A9A-B486-EC0451219335"),
        Name = "Action-RPG"
    };

    public static readonly GenreEntity FirstPersonShooter = new()
    {
        Id = Guid.Parse("26359886-E75C-4ECC-A0A1-349E4D1F11B5"),
        Name = "First Person Shooter"
    };
    
    public static readonly GenreEntity Mmorpg = new()
    {
        Id = Guid.Parse("45147E9C-B11C-42F1-87A0-368AD1A4CCDE"),
        Name = "MMORPG"
    };

    public static readonly GenreEntity OpenWorld = new()
    {
        Id = Guid.Parse("AA98D01F-CD30-4D97-BD9F-2A056042BC49"),
        Name = "Open World"
    };

    public static readonly GenreEntity Rpg = new()
    {
        Id = Guid.Parse("7679E383-E2FE-4946-860B-849587C9430F"),
        Name = "RPG"
    };

    public static DbContext SeedGenres(this DbContext dbx)
    {
        dbx.Set<GenreEntity>().AddRange(
            ActionRpg,
            FirstPersonShooter,
            Mmorpg,
            OpenWorld,
            Rpg
        );
        
        return dbx;
    }
}