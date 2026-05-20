using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.BL.Models;

namespace Launcher.App.Models;

public partial class GenreFilterItem : ObservableObject
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;

    [ObservableProperty]
    private bool _isSelected;

    public static GenreFilterItem FromModel(GenreListModel model) => new()
    {
        Id = model.Id,
        Name = model.Name
    };
}