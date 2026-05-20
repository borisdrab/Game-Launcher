namespace Launcher.App.Messages;

public record ReviewEditMessage
{
    public required Guid ReviewId { get; init; }
}
