namespace Launcher.BL.Models;

public class ReviewModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
