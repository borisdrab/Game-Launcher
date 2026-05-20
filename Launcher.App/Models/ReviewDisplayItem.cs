namespace Launcher.App.Models;

public class ReviewDisplayItem
{
    public Guid Id { get; init; }
    public Guid GameTitleId { get; init; }
    public Guid UserId { get; init; }
    public string GameName { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string? Text { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsOwnedByCurrentUser { get; init; }
}
