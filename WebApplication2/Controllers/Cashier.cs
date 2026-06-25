using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class CashierController : Controller
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
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            ViewBag.TotalTransactions = db.Transactions.Count();

            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            ViewBag.TodayTransactions = db.Transactions
                .Count(t => t.TransactionDate >= today &&
                            t.TransactionDate < tomorrow);

            ViewBag.TotalBeneficiaries = db.Beneficiaries.Count();
            

            return View();
        }

        // =========================
        // TRANSACTION PAGE
        // =========================
        public ActionResult Transactions()
        {
            var data = db.Transactions
                         .OrderByDescending(x => x.TransactionDate)
                         .ToList();

            return View(data);
        }

        

        // =========================
        // BENEFICIARIES (VIEW ONLY)
        // =========================
        public ActionResult Beneficiaries()
        {
            var data = db.Beneficiaries.ToList();
            return View(data);
        }

        // =========================
        // QUICK ACTION REDIRECTS
        // =========================
        public ActionResult Deposit()
        {
            return RedirectToAction("Create", "Transactions");
        }

        public ActionResult Withdraw()
        {
            return RedirectToAction("Create", "Transactions");
        }

        public ActionResult Transfer()
        {
            return RedirectToAction("Create", "Transactions");
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