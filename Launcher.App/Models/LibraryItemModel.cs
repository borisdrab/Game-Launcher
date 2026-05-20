using System;

namespace Launcher.App.Models;

public class LibraryItemModel
{
    public Guid Id { get; set; } // GameTitleId
    public string Name { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public int PegiRating { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public int PriceCentsAtPurchase { get; set; }
    public bool IsFavorite { get; set; }

    public string PriceInEurosText => $"{PriceCentsAtPurchase / 100.0:F2} €";
    public string FavoriteIcon => IsFavorite ? "★" : "☆";
    public string FavoriteIconColor => IsFavorite ? "#FFD700" : "#333333";
}
