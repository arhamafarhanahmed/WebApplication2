using System.Linq;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class LoginController : Controller
    {
        private BMSEntities db = new BMSEntities();

       

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            var user = db.Users
                .FirstOrDefault(x => x.Username == username &&
                                     x.PasswordHash == password);

            if (user != null)
            {
                Session["UserID"] = user.UserID;
                Session["Username"] = user.Username;
                Session["RoleID"] = user.RoleID;

                if (user.Role.RoleName == "Admin")
                {
                    Session["DashboardUrl"] = "/Admin/Dashboard";
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (user.Role.RoleName == "Manager")
                {
                    Session["DashboardUrl"] = "/Manager/Dashboard";
                    return RedirectToAction("Dashboard", "Manager");
                }
                else if (user.Role.RoleName == "Cashier")
                {
                    Session["DashboardUrl"] = "/Cashier/Dashboard";
                    return RedirectToAction("Dashboard", "Cashier");
                }
                else if (user.Role.RoleName == "Customer")
                {
                    Session["DashboardUrl"] = "/Customer/Dashboard";
                    return RedirectToAction("Dashboard", "Customer");
                }
                else if (user.Role.RoleName == "Support")
                {
                    Session["DashboardUrl"] = "/Support/Dashboard";
                    return RedirectToAction("Dashboard", "Support");
                }
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }
}