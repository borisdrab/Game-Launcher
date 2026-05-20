namespace Launcher.App.Messages;

public record ReviewDeleteMessage
{
    public required Guid ReviewId { get; init; }
}
