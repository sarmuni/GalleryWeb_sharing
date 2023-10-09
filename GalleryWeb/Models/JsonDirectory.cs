using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalleryWeb.Models
{
    public class JsonDirectory
    {
        public List<Models.DirName> data { get; set; }
        public pagination pagination { get; set; }
    }
}