namespace Launcher.DAL.Entities;

public class LibraryEntity : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    
    public UserEntity? User { get; set; }
    
    public ICollection<LibraryTitleEntity> LibraryTitles { get; set; } = new List<LibraryTitleEntity>();
    
}