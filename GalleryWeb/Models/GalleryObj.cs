using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalleryWeb.Models
{
    public class GalleryObj
    {
        public Models.Group Group { get; set; }
        public List<Models.Picture> Pictures { get; set; }
    }
}