using System.Linq;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class AdminController : Controller
    {
        private BMSEntities db = new BMSEntities();

        
        // ======================
        // AUTH CHECK
        // ======================
        private bool IsAdmin()
        {
            return Session["UserID"] != null &&
                   Session["RoleID"] != null &&
                   (int)Session["RoleID"] == 1;
        }

        private ActionResult RedirectToLogin()
        {
            return RedirectToAction("Index", "Login");
        }

        // ======================
        // ADMIN DASHBOARD
        // ======================
        public ActionResult Dashboard()
        {
            if (!IsAdmin())
                return RedirectToLogin();

            ViewBag.Username = Session["Username"];

            ViewBag.TotalUsers = db.Users.Count();
            ViewBag.TotalCustomers = db.Customers.Count();
            ViewBag.TotalAccounts = db.Accounts.Count();
            ViewBag.TotalTransactions = db.Transactions.Count();
            ViewBag.TotalLoans = db.Loans.Count();
            ViewBag.TotalEmployees = db.Employees.Count();
            ViewBag.TotalBranches = db.Branches.Count();
            ViewBag.TotalDebitCards = db.DebitCards.Count();
            return View();
        }

        // ======================
        // LOGOUT
        // ======================
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Index", "Login");
        }
    }
}