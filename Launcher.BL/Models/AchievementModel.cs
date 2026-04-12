namespace Launcher.BL.Models;

public class AchievementModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Points { get; set; }
    public int UserAchievementCount { get; set; }
    public int CompletedUsersCount { get; set; }
    public double AverageProgressPercentage { get; set; }
}
