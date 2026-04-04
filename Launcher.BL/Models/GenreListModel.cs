namespace Launcher.BL.Models;

public class GenreListModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public static GenreListModel Empty => new();
}
