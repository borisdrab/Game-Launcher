namespace Launcher.BL.Models;

public class PlatformDetailModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public static PlatformDetailModel Empty => new();
}
