namespace Launcher.BL.Models;

public class AchievementListModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Points { get; set; }

    public Guid GameTitleId { get; set; }

    public static AchievementListModel Empty => new();
}
