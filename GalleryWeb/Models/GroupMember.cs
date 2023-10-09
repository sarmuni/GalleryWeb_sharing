using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalleryWeb.Models
{
    public class GroupMember
    {
        public int GroupID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string CrtUsrID { get; set; }
        public string TsCrt { get; set; }
        public string ModUsrID { get; set; }
        public string TsMod { get; set; }
        public string ActiveFlag { get; set; }

        public string FullName { get; set; }
    }
}