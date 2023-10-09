using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalleryWeb.Models
{
    public class pagination
    {
        public int totalItems { get; set; }
        public int itemPerPage { get; set; }
        public int currentPage { get; set; }
    }
}