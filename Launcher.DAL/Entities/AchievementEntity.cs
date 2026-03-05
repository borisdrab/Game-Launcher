namespace Launcher.DAL.Entities;

public class AchievementEntity : IEntity
{
    public Guid Id { get; set; }
    
    public Guid GameTitleId { get; set; }
    public GameTitleEntity? GameTitle { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public int Points { get; set; }

    public ICollection<UserAchievementEntity> UserAchievements { get; set; } = new List<UserAchievementEntity>();
}

public class UserAchievementEntity
{
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    
    public Guid AchievementId { get; set; }
    public AchievementEntity? Achievement { get; set; }
    
    public DateTime? UnlockedAt { get; set; }
    
    public int ProgressPercentage { get; set; }
    
}