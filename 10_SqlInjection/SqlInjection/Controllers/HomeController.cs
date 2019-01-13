using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SqlInjection.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Feedback()
        {
            return View();
        }


        [HttpPost]
        public ActionResult DoLogin()
        {
            var username = Request["username"];
            var password = Request["password"];

            SqlConnection con = new SqlConnection();
            con.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\MEINEDATEN\\DOCUMENTS\\SQL_XSS_INJECTION.MDF;Integrated Security=True;Connect Timeout=30";

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "SELECT [Id], [username], [password] FROM [dbo].[User] WHERE [username] = '" + username + "' AND [password] = '" + password + "'";
            cmd.Connection = con;

            con.Open();

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                ViewBag.Message = "success";

                while (reader.Read())
                {
                    ViewBag.Message += reader.GetInt32(0) + " " + reader.GetString(1) + " " + reader.GetString(2);
                }
            }
            else
            {
                ViewBag.Message = "nothing to read of";
            }
            return View("Login");
        }

        [HttpPost]
        public ActionResult DoFeedback()
        {
            var feedback = Request["feedback"];

            SqlConnection con = new SqlConnection();
            con.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\MEINEDATEN\\DOCUMENTS\\SQL_XSS_INJECTION.MDF;Integrated Security=True;Connect Timeout=30";

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "INSERT INTO [dbo].[Feedback] SET [feedback] = '" + feedback + "'";
            cmd.Connection = con;

            con.Open();

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                ViewBag.Message = "success";

                while (reader.Read())
                {
                    ViewBag.Message += reader.GetInt32(0) + " " + reader.GetString(1) + " " + reader.GetString(2);
                }
            }
            else
            {
                ViewBag.Message = "nothing to read of";
            }
            return View("Feedback");
        }
    }
}