namespace Launcher.DAL.Entities;

public class GameTitleEntity : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public int PegiRating { get; set; }
    public int PriceCents { get; set; }
    
    public string CoverImageUrl { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    
    public DateTime? ReleaseDate  { get; set; }
    public bool IsAvailable { get; set; } = true;
    
    public ICollection<LibraryTitleEntity> LibraryTitles { get; set; } = new List<LibraryTitleEntity>();
    public ICollection<ReviewEntity> Reviews { get; set; } = new List<ReviewEntity>();
    public ICollection<GameTitlePlatformEntity> GameTitlePlatforms { get; set; } = new List<GameTitlePlatformEntity>();

    public ICollection<GameTitleGenreEntity> GameTitleGenres { get; set; } = new List<GameTitleGenreEntity>();
    public ICollection<AchievementEntity> Achievements { get; set; } = new List<AchievementEntity>();
}
