using System;

namespace Launcher.BL.Models;

public class LibraryTitleListModel
{
    public Guid LibraryId { get; set; }
    
    public Guid GameTitleId { get; set; }
    public GameTitleListModel? GameTitle { get; set; }
    
    public DateTime AddedAt { get; set; }
    public bool IsFavorite { get; set; }
    public int PriceCentsAtPurchase { get; set; } 
}
