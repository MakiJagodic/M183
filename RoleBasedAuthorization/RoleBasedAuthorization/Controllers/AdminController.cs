using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RoleBasedAuthorization.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Dashboard()
        {
            var current_user = (string)Session["username"];
            var user_roles = MvcApplication.UserRoles;
            var current_user_role = (string)user_roles[current_user];

            if (current_user_role == "Administrator")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}