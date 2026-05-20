namespace Launcher.App.Messages;

public record GameTitleEditMessage
{
    public required Guid GameTitleId { get; init; }
}