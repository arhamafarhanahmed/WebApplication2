using System.Linq;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class ManagerController : Controller
    {
        private BMSEntities db = new BMSEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserID"] == null)
            {
                filterContext.Result = RedirectToAction("Index", "Login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        // =========================
        // DASHBOARD
        // =========================
        public ActionResult Dashboard()
        {
            // SECURITY CHECK (optional but recommended)
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
            {
                return RedirectToAction("Index", "Login");
            }

            // STATS
            ViewBag.TotalCustomers = db.Customers.Count();
            ViewBag.TotalAccounts = db.Accounts.Count();
            ViewBag.TotalLoans = db.Loans.Count();
            ViewBag.TotalTransactions = db.Transactions.Count();
            ViewBag.TotalEmployees = db.Employees.Count();

            return View();
        }

        // =========================
        // CUSTOMERS
        // =========================
        public ActionResult Customers()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
                return RedirectToAction("Index", "Login");

            var data = db.Customers.ToList();
            return View(data);
        }

        // =========================
        // ACCOUNTS
        // =========================
        public ActionResult Accounts()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
                return RedirectToAction("Index", "Login");

            var data = db.Accounts.ToList();
            return View(data);
        }

        // =========================
        // LOANS
        // =========================
        public ActionResult Loans()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
                return RedirectToAction("Index", "Login");

            var data = db.Loans.ToList();
            return View(data);
        }

        // =========================
        // TRANSACTIONS
        // =========================
        public ActionResult Transactions()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
                return RedirectToAction("Index", "Login");

            var data = db.Transactions.ToList();
            return View(data);
        }

        // =========================
        // EMPLOYEES
        // =========================
        public ActionResult Employees()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
                return RedirectToAction("Index", "Login");

            var data = db.Employees.ToList();
            return View(data);
        }

        // =========================
        // LOGOUT
        // =========================
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Index", "Login");
        }
    }
}