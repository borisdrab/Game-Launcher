namespace Launcher.BL.Models;

public class GameTitleListModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public int PegiRating { get; set; }
    public int PriceCents { get; set; }

    public string CoverImageUrl { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;

    public DateTime? ReleaseDate { get; set; }
    public bool IsAvailable { get; set; }

    public static GameTitleListModel Empty => new();
}