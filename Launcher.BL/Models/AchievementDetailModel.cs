namespace Launcher.BL.Models;

public class AchievementDetailModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Points { get; set; }

    public Guid GameTitleId { get; set; }

    public static AchievementDetailModel Empty => new();
}
