namespace Launcher.BL.Models;

public class QueryObject
{
    // Text to search for (e.g. "RPG" to find genres containing "RPG")
    public string? SearchTerm { get; set; }

    // Which property to sort by (e.g. "Name")
    public string? SortBy { get; set; }

    // If true, sort Z-A instead of A-Z
    public bool SortDescending { get; set; } = false;
}
