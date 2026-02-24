namespace Launcher.DAL.Entities;

public class GameTitleEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public int PegiRating { get; set; }
    
    public ICollection<LibraryTitleEntity> LibraryTitles { get; set; } = new List<LibraryTitleEntity>();
}