using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private BMSEntities db = new BMSEntities();

        

        // 🏠 PUBLIC LANDING PAGE (NO DB ACCESS)
        public ActionResult Index()
        {
            return View();
        }

        // 🔐 ROLE BASED DASHBOARD (LOGIN REQUIRED)
        public ActionResult Home()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                int roleId = Convert.ToInt32(Session["RoleID"]);

                // 🔹 ADMIN / MANAGER FULL ACCESS
                if (roleId == 1 || roleId == 2)
                {
                    ViewBag.TotalUsers = db.Users.Count();
                    ViewBag.TotalCustomers = db.Customers.Count();
                    ViewBag.TotalAccounts = db.Accounts.Count();
                    ViewBag.TotalTransactions = db.Transactions.Count();
                    ViewBag.TotalLoans = db.Loans.Count();
                }

                // 🔹 CASHIER LIMITED ACCESS
                else if (roleId == 3)
                {
                    ViewBag.TotalUsers = db.Users.Count();
                    ViewBag.TotalTransactions = db.Transactions.Count();

                    ViewBag.TotalCustomers = "-";
                    ViewBag.TotalAccounts = "-";
                    ViewBag.TotalLoans = "-";
                }

                // 🔹 CUSTOMER OWN DATA ONLY
                else if (roleId == 4)
                {
                    int userId = Convert.ToInt32(Session["UserID"]);

                    var customer = db.Customers.FirstOrDefault(x => x.UserID == userId);

                    if (customer != null)
                    {
                        ViewBag.CustomerName = customer.FirstName + " " + customer.LastName;
                    }

                    ViewBag.TotalUsers = "-";
                    ViewBag.TotalCustomers = "-";
                    ViewBag.TotalAccounts = "-";
                    ViewBag.TotalTransactions = "-";
                    ViewBag.TotalLoans = "-";
                }

                ViewBag.Username = Session["Username"];
                ViewBag.RoleID = roleId;
            }
            catch (Exception)
            {
                ViewBag.TotalUsers = 0;
                ViewBag.TotalCustomers = 0;
                ViewBag.TotalAccounts = 0;
                ViewBag.TotalTransactions = 0;
                ViewBag.TotalLoans = 0;
            }

            return View();
        }

        // 🚪 LOGOUT
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}