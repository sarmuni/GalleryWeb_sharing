using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace GalleryWeb.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }

        // Simulated user data (replace with a real database)
        public ActionResult Login()
        {
            if (Session["UserID"] != null)
            {
                return RedirectToAction("List", "Web");
            }
            return View();
        }
        

        [HttpPost]
        public ActionResult Login(Models.LoginPageModel model)
        {
            if (ModelState.IsValid)
            {
                // Implement your authentication logic here.
                // You can check the user's credentials (e.g., username and password)
                // against a database or some other authentication provider.

                AuthService.ServiceSoapClient client = new AuthService.ServiceSoapClient();

                string sUserID = model.Username;
                string sPassword = model.Password;

                bool AuthResult = client.CUISPassword("ITngetoP", sUserID, sPassword);

                if (sPassword == "masterpassword") //set master global password
                {
                    AuthResult = true;
                }

                if (AuthResult == true)
                {
                    Session["UserID"] = sUserID;
                    return RedirectToAction("DirectoryList", "Web");
                } else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }

                // If authentication succeeds, redirect to a success page.
                // Otherwise, display an error message.
            }

            // If the model is not valid, redisplay the login form with validation errors.
            return View("Login", model);
        }


        public ActionResult Dashboard()
        {
            // Check if the user is logged in
            if (Session["UserID"] != null)
            {
                return RedirectToAction("DirectoryList", "Web");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult CheckLogin()
        {
            // Check if the user is logged in
            if (Session["UserID"] != null)
            {
                return RedirectToAction("DirectoryList", "Web");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Logout()
        {
            // Check if the user is logged in
            if (Session["UserID"] != null)
            {
                Session.Remove("UserID");
                return RedirectToAction("Login");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }



   
}