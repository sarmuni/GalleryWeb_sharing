using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalleryWeb.Models
{
    public class Picture
    {
        public int ID { get; set; }
        public string DirName { get; set; }
        public string CaptureFile { get; set; }
        public string CrtUsrID { get; set; }
        public string TsCrt { get; set; }
        public string ModUsr { get; set; }
        public string TsMod { get; set; }
        public string ActiveFlag { get; set; }
    }
}