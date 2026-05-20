namespace Launcher.BL.Models;

public class QueryObject
{
    public string? SearchTerm { get; set; }

    public string? SortBy { get; set; }

    public bool SortDescending { get; set; } = false;
}
