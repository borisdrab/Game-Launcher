using CommunityToolkit.Mvvm.ComponentModel;

namespace Launcher.App.Models;

public partial class SelectableItem : ObservableObject
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;

    [ObservableProperty]
    private bool _isSelected;
}