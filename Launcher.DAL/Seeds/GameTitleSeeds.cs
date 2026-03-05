using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.DAL.Seeds;

public static class GameTitleSeeds
{
    public static readonly GameTitleEntity TheWitcher3 = new()
    {
        Id = Guid.Parse("AA83E61D-D3C1-4F40-A724-797324749B5E"),
        Name = "The Witcher 3: Wild Hunt",
        Description = "You are Geralt of Rivia, mercenary monster slayer. Before you stands a war-torn, monster-infested continent you can explore at will. Your current contract? Tracking down Ciri — the Child of Prophecy, a living weapon that can alter the shape of the world.",
        PegiRating = 18,
        PriceCents = 2999,
        ReleaseDate = new DateTime(2015, 5, 18),
        Publisher = "CD PROJECT RED",
        IsAvailable = true,
        CoverImageUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/0/0c/Witcher_3_cover_art.jpg/250px-Witcher_3_cover_art.jpg"
    };

    public static readonly GameTitleEntity EldenRing = new()
    {
        Id = Guid.Parse("C35111AC-244C-4C1C-8882-1E57C50622DC"),
        Name = "ELDEN RING",
        Description = "THE CRITICALLY ACCLAIMED FANTASY ACTION RPG. Rise, Tarnished, and be guided by grace to brandish the power of the Elden Ring and become an Elden Lord in the Lands Between.",
        PegiRating = 16,
        PriceCents = 5999,
        ReleaseDate = new DateTime(2022, 2, 25),
        Publisher = "Bandai Namco Entertainment",
        IsAvailable = true,
        CoverImageUrl = "https://m.media-amazon.com/images/I/6110RSDn3PL._AC_UF350,350_QL80_.jpg"
    };

    public static DbContext SeedGameTitles(this DbContext dbx)
    {
        dbx.Set<GameTitleEntity>().AddRange(
            TheWitcher3,
            EldenRing
        );
        
        return dbx;
    }
}