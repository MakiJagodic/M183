using logging_audit_trails.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace logging_audit_trails.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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

        [HttpPost]
        public ActionResult DoLogin()
        {
            var username = Request["username"];
            var password = Request["password"];

            var ip = Request.ServerVariables["REMOTE_ADDR"];
            var platform = Request.Browser.Platform;
            var browser = Request.UserAgent;

            SqlConnection con = new SqlConnection(); 
            con.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\MEINEDATEN\\DOCUMENTS\\LOGGING_INTRUSION_DETECTION.MDF;Integrated Security=True;Connect Timeout=30";

            SqlCommand cmd_credentials = new SqlCommand();
            cmd_credentials.CommandText = "SELECT [Id] FROM [dbo].[User] WHERE [Username] = '" + username + "' AND [Password] = '" + password + "'";
            cmd_credentials.Connection = con;

            con.Open();

            SqlDataReader reader_credentials = cmd_credentials.ExecuteReader();

            if (reader_credentials.HasRows)
            {
                var user_id = 0;
                while (reader_credentials.Read())
                {
                    user_id = reader_credentials.GetInt32(0);
                    break;
                }

                con.Close();
                con.Open();

                SqlCommand cmd_user_using_usual_browser = new SqlCommand();
                cmd_user_using_usual_browser.CommandText = "SELECT [Id] FROM [dbo].[User-Log] WHERE [UserId] = '" + user_id + "' AND [IP] LIKE '" + ip.Substring(0, 2) + "%' AND [browser] LIKE '" + platform + "%'";
                cmd_user_using_usual_browser.Connection = con;

                SqlDataReader reader_usual_browser = cmd_user_using_usual_browser.ExecuteReader();

                if (!reader_usual_browser.HasRows)
                {
                    con.Close();
                    con.Open();

                    SqlCommand log_cmd = new SqlCommand();
                    log_cmd.CommandText = "INSERT INTO [dbo].[User-Log] (UserId, IP, Action, Result, CreatedOn, Browser, AdditionalInformation) VALUES('" + user_id + "', '" + ip + "', 'login', 'success', GETDATE(), '" + platform + "', 'other browser')";
                    log_cmd.Connection = con;
                    log_cmd.ExecuteReader();
                }
                else
                {
                    con.Close();
                    con.Open();

                    SqlCommand log_cmd = new SqlCommand();
                    log_cmd.CommandText = "INSERT INTO [dbo].[User-Log] (UserId, IP, Action, Result, CreatedOn, Browser) VALUES('" + user_id + "', '" + ip + "', 'login', 'success', GETDATE(), '" + platform + "')";
                    log_cmd.Connection = con;
                    log_cmd.ExecuteReader();
                }
            }
            else
            {
                con.Close();
                con.Open();

                SqlCommand cmd_userid_by_name = new SqlCommand();

                cmd_userid_by_name.CommandText = "SELECT [Id] FROM [dbo].[User] WHERE [Username] = '" + username + "'";
                cmd_userid_by_name.Connection = con;

                SqlDataReader reader_userid_by_name = cmd_userid_by_name.ExecuteReader();

                if (reader_userid_by_name.HasRows)
                {
                    var user_id = 0;
                    while (reader_userid_by_name.Read())
                    {
                        user_id = reader_userid_by_name.GetInt32(0);
                        break;
                    }

                    con.Close();
                    con.Open();

                    SqlCommand failed_log_cmd = new SqlCommand();
                    failed_log_cmd.CommandText = "SELECT COUNT(ID) FROM [dbo].[User-Log] WHERE UserId = '" + user_id + "' AND RESULT = 'failed' AND CAST(CreatedOn As date) = '" + System.DateTime.Now.ToShortDateString().Substring(0, 10) + "'";
                    failed_log_cmd.Connection = con;
                    SqlDataReader failed_login_count = failed_log_cmd.ExecuteReader();

                    var attempts = 0;
                    if (failed_login_count.HasRows)
                    {
                        while (failed_login_count.Read())
                        {
                            attempts = failed_login_count.GetInt32(0);
                            break;
                        }
                    }

                    if (attempts >= 5 || password.Length < 4 || password.Length > 20)
                    {
                        //block user
                    }

                    con.Close();
                    con.Open();

                    SqlCommand log_cmd = new SqlCommand();
                    log_cmd.CommandText = "INSERT INTO [dbo].[User-Log] (UserId, IP, Action, Result, CreatedOn, Browser) VALUES('" + user_id + "', '" + ip + "', 'login', 'failed', GETDATE(), '" + platform + "')";
                    log_cmd.Connection = con;
                    log_cmd.ExecuteReader();

                    ViewBag.Message = "No user found";
                }
                else
                {
                    con.Close();
                    con.Open();

                    SqlCommand log_cmd = new SqlCommand();
                    log_cmd.CommandText = "INSERT INTO [dbo].[User-Log] (UserId, IP, Action, Result, CreatedOn, AdditionalInformation, Browser) VALUES(0, '" + ip + "', 'login', 'failed', GETDATE(), 'No User Found', '" + platform + "')";
                    log_cmd.Connection = con;
                    log_cmd.ExecuteReader();

                    ViewBag.Message = "No User Found";
                }
            }

            con.Close();

            return RedirectToAction("Logs", "Home");
        }

        public ActionResult Logs()
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\MEINEDATEN\\DOCUMENTS\\LOGGING_INTRUSION_DETECTION.MDF;Integrated Security=True;Connect Timeout=30";

            SqlCommand cmd_credentials = new SqlCommand();
            cmd_credentials.CommandText = "SELECT * FROM [dbo].[User-Log] ul JOIN [dbo].[User] u ON ul.UserId = u.Id ORDER BY ul.CreatedOn DESC";
            cmd_credentials.Connection = con;

            con.Open();

            SqlDataReader reader = cmd_credentials.ExecuteReader();

            if (reader.HasRows)
            {
                List<HomeControllerViewModel> model = new List<HomeControllerViewModel>();
                while (reader.Read())
                {
                    var log_entry = new HomeControllerViewModel();
                    log_entry.UserId = reader.GetValue(10).ToString();
                    log_entry.LogId = reader.GetValue(0).ToString();
                    log_entry.LogCreatedOn = reader.GetValue(7).ToString();

                    model.Add(log_entry);
                }

                return View(model);
            }
            else
            {
                ViewBag.Message = "No Results found";
                return View();
            }
        }
    }
}