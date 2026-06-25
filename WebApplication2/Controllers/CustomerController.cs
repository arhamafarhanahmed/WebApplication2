using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using System.Data.Entity;

namespace WebApplication2.Controllers
{
    public class CustomerController : Controller
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
            int userId = int.Parse(Session["UserID"].ToString());

            var customer = db.Customers
                             .FirstOrDefault(c => c.UserID == userId);

            if (customer == null)
                return RedirectToAction("Index", "Login");

            ViewBag.CustomerName = customer.FirstName + " " + customer.LastName;

            ViewBag.TotalAccounts = db.Accounts
                                      .Count(a => a.CustomerID == customer.CustomerID);

            ViewBag.TotalLoans = db.Loans
                                   .Count(l => l.CustomerID == customer.CustomerID);

            ViewBag.TotalBeneficiaries = db.Beneficiaries
                                           .Count(b => b.CustomerID == customer.CustomerID);

            ViewBag.TotalCards =
                (from dc in db.DebitCards
                 join a in db.Accounts on dc.AccountID equals a.AccountID
                 where a.CustomerID == customer.CustomerID
                 select dc).Count();

            return View();
        }

        // =========================
        // UPLOAD DOCUMENT
        // =========================

        public ActionResult UploadDocument()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadDocument(string documentType, HttpPostedFileBase file)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            int userId = int.Parse(Session["UserID"].ToString());

            var customer = db.Customers
                             .FirstOrDefault(c => c.UserID == userId);

            if (customer == null)
                return RedirectToAction("Dashboard");

            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);

                string folder = Server.MapPath("~/Uploads/");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string path = Path.Combine(folder, fileName);

                file.SaveAs(path);

                CustomerDocument doc = new CustomerDocument();

                doc.CustomerID = customer.CustomerID;
                doc.DocumentType = documentType;
                doc.FileName = fileName;
                doc.FilePath = "/Uploads/" + fileName;
                doc.UploadDate = DateTime.Now;

                db.CustomerDocuments.Add(doc);
                db.SaveChanges();

                ViewBag.Message = "Document Uploaded Successfully";
            }

            return View();
        }

        // =========================
        // MY ACCOUNT
        // =========================
        public ActionResult MyAccount()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            int userId = int.Parse(Session["UserID"].ToString());

            var customer = db.Customers
                             .FirstOrDefault(c => c.UserID == userId);

            return View(customer);
        }

        // =========================
        // BALANCE
        // =========================
        public ActionResult Balance()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            int userId = int.Parse(Session["UserID"].ToString());

            var customer = db.Customers
                             .FirstOrDefault(c => c.UserID == userId);

            if (customer == null)
                return RedirectToAction("Dashboard");

            var accounts = db.Accounts
                             .Where(a => a.CustomerID == customer.CustomerID)
                             .ToList();

            return View(accounts);
        }
        // =========================
        // TRANSACTIONS
        // =========================
        public ActionResult Transactions()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            int userId = int.Parse(Session["UserID"].ToString());

            var customer = db.Customers
                             .FirstOrDefault(c => c.UserID == userId);

            if (customer == null)
                return RedirectToAction("Dashboard");

            var transactions =
                (from t in db.Transactions
                 join a in db.Accounts on t.AccountID equals a.AccountID
                 where a.CustomerID == customer.CustomerID
                 orderby t.TransactionDate descending
                 select t).ToList();

            return View(transactions);
        }

        // =========================
        // DEBIT CARDS
        // =========================


        public ActionResult DebitCards()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            int userId = Convert.ToInt32(Session["UserID"]);

            var customer = db.Customers
                             .FirstOrDefault(c => c.UserID == userId);

            if (customer == null)
            {
                TempData["Error"] = "Customer record not found.";
                return RedirectToAction("Dashboard", "Customer");
            }

            var cards = (from dc in db.DebitCards
                         join a in db.Accounts
                         on dc.AccountID equals a.AccountID
                         where a.CustomerID == customer.CustomerID
                         select dc).ToList();

            return View(cards);
        }
        // =========================
        // LOANS
        // =========================
        public ActionResult Loans()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            var customer = db.Customers
                             .FirstOrDefault(c => c.UserID == userId);

            var loans = db.Loans
                          .Include("LoanType")
                          .Where(l => l.CustomerID == customer.CustomerID)
                          .ToList();

            return View(loans);
        }
        // =========================
        // BENEFICIARIES
        // =========================
        public ActionResult Beneficiaries()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            int userId = int.Parse(Session["UserID"].ToString());

            var customer = db.Customers
                             .FirstOrDefault(c => c.UserID == userId);

            var beneficiaries = db.Beneficiaries
                                  .Where(b => b.CustomerID == customer.CustomerID)
                                  .ToList();

            return View(beneficiaries);
        }


        public ActionResult ReportCard(int id)
        {
            var card = db.DebitCards.Find(id);

            if (card != null && card.Status == "Blocked")
            {
                card.Status = "Reported";
                db.SaveChanges();
            }

            return RedirectToAction("DebitCards");
        }
        public ActionResult CardIssues()
        {
            var cards = db.DebitCards
                          .Where(x => x.Status == "Blocked")
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