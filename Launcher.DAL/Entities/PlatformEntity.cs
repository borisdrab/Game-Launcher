namespace Launcher.DAL.Entities;

public class PlatformEntity : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<GameTitlePlatformEntity> GameTitlePlatforms { get; set; } = new List<GameTitlePlatformEntity>();
}

public class GameTitlePlatformEntity
{
    public Guid GameTitleId { get; set; }
    public GameTitleEntity? GameTitle { get; set; }
    
    public Guid PlatformId { get; set; }
    public PlatformEntity? Platform { get; set; }
}