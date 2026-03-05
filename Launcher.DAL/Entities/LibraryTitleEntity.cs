namespace Launcher.DAL.Entities;

public class LibraryTitleEntity
{
    public Guid LibraryId { get; set; }
    public LibraryEntity? Library { get; set; }
    
    public Guid GameTitleId { get; set; }
    public GameTitleEntity? GameTitle { get; set; }
    
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public bool IsFavorite { get; set; }

    public int PriceCentsAtPurchase { get; set; } 
}