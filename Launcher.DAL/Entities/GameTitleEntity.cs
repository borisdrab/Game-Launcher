namespace Launcher.DAL.Entities;

public class GameTitleEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public int PegiRating { get; set; }
    public int PriceCents { get; set; }
    
    public string CoverImageUrl { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    //public string Developer { get; set; } = string.Empty;
    
    public DateTime? ReleaseDate  { get; set; }
    public bool IsAvailable { get; set; } = true;
    
    public Guid? GenreId { get; set; }
    public GenreEntity? Genre { get; set; }
    
    public ICollection<LibraryTitleEntity> LibraryTitles { get; set; } = new List<LibraryTitleEntity>();
    public ICollection<ReviewEntity> Reviews { get; set; } = new List<ReviewEntity>();
    public ICollection<GameTitlePlatformEntity> GameTitlePlatforms { get; set; } = new List<GameTitlePlatformEntity>();
    public ICollection<AchievementEntity> Achievements { get; set; } = new List<AchievementEntity>();
}
