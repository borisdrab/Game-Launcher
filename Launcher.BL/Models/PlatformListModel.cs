namespace Launcher.BL.Models;

public class PlatformListModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public static PlatformListModel Empty => new();
}
