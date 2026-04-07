using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace Launcher.BL.Models;

public class UserListModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;

    public static UserListModel Empty => new();
}