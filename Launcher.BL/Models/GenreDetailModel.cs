namespace Launcher.BL.Models;

public class GenreDetailModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public static GenreDetailModel Empty => new();
}
