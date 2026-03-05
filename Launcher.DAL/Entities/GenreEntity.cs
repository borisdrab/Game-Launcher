namespace Launcher.DAL.Entities;

public class GenreEntity : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<GameTitleGenreEntity> GamesTitleGenres { get; set; } = new List<GameTitleGenreEntity>();
}