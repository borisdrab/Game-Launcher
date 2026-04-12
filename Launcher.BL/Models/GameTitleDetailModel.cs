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
    
    public List<GenreModel> Genres { get; set; } = new();
    public List<PlatformModel> Platforms { get; set; } = new();
    public List<AchievementModel> Achievements { get; set; } = new();
    public List<ReviewModel> Reviews { get; set; } = new();

    public int AchievementCount { get; set; }
    public int ReviewCount { get; set; }
    public double? AverageRating { get; set; }
    
    public static GameTitleDetailModel Empty => new();
}