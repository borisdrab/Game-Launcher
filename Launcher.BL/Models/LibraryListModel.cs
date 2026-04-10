using Launcher.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Launcher.BL.Models
{
    public class LibraryListModel : ModelBase
    {
        public string Name { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }


        public ICollection<LibraryTitleEntity> LibraryTitles { get; set; } = new List<LibraryTitleEntity>();

        public static LibraryListModel Empty => new();
    }
}
