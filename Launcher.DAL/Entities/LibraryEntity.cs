namespace Launcher.DAL.Entities;

public class LibraryEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    
    // Foreign key na User
    public Guid UserId { get; set; }
    
    // Navigačná vlastnosť
    public UserEntity? User { get; set; }
    
    public ICollection<LibraryTitleEntity> LibraryTitles { get; set; } = new List<LibraryTitleEntity>();
    
}