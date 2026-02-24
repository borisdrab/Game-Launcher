namespace Launcher.DAL.Entities;

public class UserEntity
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    
    // Jeden user môže mať viacero knižníc
    public ICollection<LibraryEntity> Libraries { get; set; } = new List<LibraryEntity>();
}