using System.Linq;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class SupportController : Controller
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

        public ActionResult ResolveCard(int id)
        {
            var card = db.DebitCards.FirstOrDefault(x => x.CardID == id);

            if (card != null)
            {
                card.Status = "Active";
                db.SaveChanges();
            }

            return RedirectToAction("CardIssues");
        }

        // =========================
        // DASHBOARD
        // =========================

        public ActionResult Dashboard()
        {

            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            ViewBag.TotalCustomers = db.Customers.Count();

            ViewBag.TotalAccounts = db.Accounts.Count();

            ViewBag.TotalDocuments = db.CustomerDocuments.Count();

            ViewBag.TotalCards = db.DebitCards.Count();

            return View();
        }

        // =========================
        // CUSTOMERS
        // =========================
        public ActionResult Customers()
        {
            var data = db.Customers.ToList();
            return View(data);
        }

        // =========================
        // ACCOUNTS
        // =========================
        public ActionResult Accounts()
        {
            var data = db.Accounts.ToList();
            return View(data);
        }

        // =========================
        // DOCUMENTS
        // =========================
        public ActionResult Documents()
        {
            var data = db.CustomerDocuments.ToList();
            return View(data);
        }

        public ActionResult CardIssues()
        {
            var cards = db.DebitCards
                          .Where(x => x.Status == "Reported")
                          .ToList();

            return View(cards);
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