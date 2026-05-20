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
        public UserListModel? User { get; set; }

        public ICollection<LibraryTitleListModel> LibraryTitles { get; set; } = new List<LibraryTitleListModel>();

        public static LibraryListModel Empty => new();
    }
}
