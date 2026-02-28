namespace Launcher.DAL.Entities;

public class LibraryTitleEntity
{
    // FK na Library
    public Guid LibraryId { get; set; }
    public LibraryEntity? Library { get; set; }
    
    // FK na GameTitle
    public Guid GameTitleId { get; set; }
    public GameTitleEntity? GameTitle { get; set; }
    
    // bonus, hodí sa pre UI aj sort/filter
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsFavorite { get; set; }
}