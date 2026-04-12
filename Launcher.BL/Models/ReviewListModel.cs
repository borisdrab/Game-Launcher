namespace Launcher.BL.Models;

public class ReviewListModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid GameTitleId { get; set; }

    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; }

    public static ReviewListModel Empty => new();
}
