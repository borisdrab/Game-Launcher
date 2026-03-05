namespace Launcher.DAL.Entities;

public class ReviewEntity : IEntity
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    
    public Guid GameTitleId { get; set; }
    public GameTitleEntity? GameTitle { get; set; }
    
    public int Rating { get; set; }
    
    public string? Text { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
}