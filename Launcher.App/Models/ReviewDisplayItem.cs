namespace Launcher.App.Models;

// Helper model for displaying reviews in the Socials list.
// Combines review data with the game name and user display name for easy binding.
public class ReviewDisplayItem
{
    public Guid Id { get; init; }
    public Guid GameTitleId { get; init; }
    public Guid UserId { get; init; }
    public string GameName { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public DateTime CreatedAt { get; init; }
}
