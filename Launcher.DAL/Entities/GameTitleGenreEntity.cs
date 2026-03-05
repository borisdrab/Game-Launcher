namespace Launcher.DAL.Entities;

public class GameTitleGenreEntity
{
    public Guid GameTitleId { get; set; }
    public GameTitleEntity? GameTitle { get; set; }
    
    public Guid GenreId { get; set; }
    public GenreEntity? Genre { get; set; }
}