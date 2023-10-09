using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GalleryWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Check if the user is logged in
            if (Session["UserID"] != null)
            {
                return RedirectToAction("DirectoryList", "Web");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}