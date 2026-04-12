namespace Launcher.BL.Models;

public class GameTitleDetailModel : ModelBase
{

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int PegiRating { get; set; }
    public int PriceCents { get; set; }

    public string CoverImageUrl { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;

    public DateTime? ReleaseDate { get; set; }
    public bool IsAvailable { get; set; }
    
    public List<GenreDetailModel> Genres { get; set; } = new();
    public List<PlatformDetailModel> Platforms { get; set; } = new();
    public List<AchievementDetailModel> Achievements { get; set; } = new();
    public List<ReviewDetailModel> Reviews { get; set; } = new();

    public int AchievementCount { get; set; }
    public int ReviewCount { get; set; }
    public double? AverageRating { get; set; }
    
    public static GameTitleDetailModel Empty => new();
}