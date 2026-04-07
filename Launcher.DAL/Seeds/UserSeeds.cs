using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.DAL.Seeds;

public static class UserSeeds
{
    public static readonly UserEntity Stepan = new()
    {
        Id = Guid.Parse("9C971563-5DD9-4A35-85F5-AEEFEE1DD0C0"),
        UserName = "Štěpán",
        Email = "xkrenos00@vutbr.cz",
        DisplayName = "stepkren",
        AvatarUrl = "https://images.steamusercontent.com/ugc/2465228135162080018/A30967B6A19D25D2368D8BF161B590B59733F717/?imw=512&&ima=fit&impolicy=Letterbox&imcolor=%23000000&letterbox=false"
    };

    public static readonly UserEntity Boris = new()
    {
        Id = Guid.Parse("006F0125-5547-4BDB-9AD1-5217AC2209FD"),
        UserName = "Boris",
        Email = "xdrabbo00@vutbr.cz",
        DisplayName = "Boris Godunov",
        AvatarUrl = "https://upload.wikimedia.org/wikipedia/commons/5/54/Boris_Godunov_by_anonim_%2817th_c.%2C_GIM%29.jpg"
    };
    
    public static readonly UserEntity Ondrej = new()
    {
        Id = Guid.Parse("06EBF092-FCDF-49FF-B211-238B11178A76"),
        UserName = "Ondrej",
        Email = "xroharo00@vutbr.cz",
        DisplayName = "Ondrjuu",
        AvatarUrl = "https://cdn.myshoptet.com/usr/eshop.moravskamincovna.cz/user/shop/big/8115-1_superjmeno-ondra-f-proweb.jpg?60bf2731"
    };
    
    public static readonly UserEntity Samuel = new()
    {
        Id = Guid.Parse("25A19BA5-FA40-459D-A4FF-B22A97EE9BB2"),
        UserName = "Samuel",
        Email = "xvajdas00@vutbr.cz",
        DisplayName = "Zd3ils",
        AvatarUrl = "https://www.nekupto.cz/user/categories/orig/samuel.png"
    };
    
    public static readonly UserEntity Jan = new()
    {
        Id = Guid.Parse("FE878FB1-76D7-4CCA-AD93-98CE8E19421A"),
        UserName = "Jan",
        Email = "xspacej00@vutbr.cz",
        DisplayName = "Schpagy",
        AvatarUrl = "https://upload.wikimedia.org/wikipedia/commons/5/5d/Bengal_cat1.jpg"
    };

    public static DbContext SeedUsers(this DbContext dbx)
    {
        dbx.Set<UserEntity>().AddRange(
            new UserEntity { Id = Stepan.Id, UserName = Stepan.UserName, Email = Stepan.Email, DisplayName = Stepan.DisplayName, AvatarUrl = Stepan.AvatarUrl },
            new UserEntity { Id = Boris.Id, UserName = Boris.UserName, Email = Boris.Email, DisplayName = Boris.DisplayName, AvatarUrl = Boris.AvatarUrl },
            new UserEntity { Id = Ondrej.Id, UserName = Ondrej.UserName, Email = Ondrej.Email, DisplayName = Ondrej.DisplayName, AvatarUrl = Ondrej.AvatarUrl },
            new UserEntity { Id = Samuel.Id, UserName = Samuel.UserName, Email = Samuel.Email, DisplayName = Samuel.DisplayName, AvatarUrl = Samuel.AvatarUrl },
            new UserEntity { Id = Jan.Id, UserName = Jan.UserName, Email = Jan.Email, DisplayName = Jan.DisplayName, AvatarUrl = Jan.AvatarUrl }
        );

        return dbx;
    }
}