namespace Launcher.BL.Models;

public class UserDetailModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;

    public static UserDetailModel Empty => new();
}