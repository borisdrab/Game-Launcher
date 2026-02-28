namespace Launcher.DAL.Entities;

public class GenreEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<GameTitleEntity> Games { get; set; } = new List<GameTitleEntity>();
}