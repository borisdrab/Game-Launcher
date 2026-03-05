namespace Launcher.DAL.Entities;

public class UserEntity : IEntity
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public string DisplayName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    
    public ICollection<LibraryEntity> Libraries { get; set; } = new List<LibraryEntity>();
    public ICollection<ReviewEntity> Reviews { get; set; } = new List<ReviewEntity>();
    public ICollection<UserAchievementEntity> UserAchievements { get; set; } = new List<UserAchievementEntity>();
}
