using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class TransactionsController : Controller
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

        public ActionResult GenerateSlip()
        {
            var transactions = db.Transactions
                                 .OrderByDescending(t => t.TransactionDate)
                                 .ToList();

            return View(transactions);
        }

       
        public ActionResult Slip(int id)
        {
            var transaction = db.Transactions.Find(id);

            if (transaction == null)
                return HttpNotFound();

            return View(transaction);
        }
        // =========================
        // HISTORY
        // =========================
        public ActionResult History()
        {
            var data = db.Transactions
                .Include(t => t.Account)
                .Include(t => t.TransactionType)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            return View(data);
        }

        // =========================
        // INDEX
        // =========================
        public ActionResult Index()
        {
            return View();
        }

        // =========================
        // CREATE GET
        // =========================
        public ActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // =========================
        // CREATE POST (FIXED)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Transaction transaction, int? BeneficiaryID)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(transaction);
            }

            var account = db.Accounts.Find(transaction.AccountID);

            if (account == null)
            {
                ModelState.AddModelError("", "Account not found");
                LoadDropdowns();
                return View(transaction);
            }

            // =========================
            // DEPOSIT
            // =========================
            if (transaction.TransactionTypeID == 1)
            {
                account.Balance += transaction.Amount;
            }

            // =========================
            // WITHDRAW
            // =========================
            else if (transaction.TransactionTypeID == 2)
            {
                if (account.Balance < transaction.Amount)
                {
                    ModelState.AddModelError("", "Insufficient Balance");
                    LoadDropdowns();
                    return View(transaction);
                }

                account.Balance -= transaction.Amount;
            }

            // =========================
            // TRANSFER (BENEFICIARY BASED)
            // =========================
            else if (transaction.TransactionTypeID == 3)
            {
                var beneficiary = db.Beneficiaries
                    .Include(b => b.Account)
                    .FirstOrDefault(b => b.BeneficiaryID == BeneficiaryID);

                if (beneficiary == null || beneficiary.Account == null)
                {
                    ModelState.AddModelError("", "Beneficiary not found");
                    LoadDropdowns();
                    return View(transaction);
                }

                var toAccount = beneficiary.Account;

                if (account.Balance < transaction.Amount)
                {
                    ModelState.AddModelError("", "Insufficient Balance");
                    LoadDropdowns();
                    return View(transaction);
                }

                account.Balance -= transaction.Amount;
                toAccount.Balance += transaction.Amount;
            }

            // =========================
            // SAVE TRANSACTION
            // =========================
            transaction.TransactionDate = DateTime.Now;

            db.Transactions.Add(transaction);
            db.SaveChanges();

            return RedirectToAction("History");
        }

        // =========================
        // DETAILS
        // =========================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var data = db.Transactions
                .Include(t => t.Account)
                .Include(t => t.TransactionType)
                .FirstOrDefault(t => t.TransactionID == id);

            if (data == null)
                return HttpNotFound();

            return View(data);
        }

        // =========================
        // DELETE (SAFE FIX)
        // =========================
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var data = db.Transactions.Find(id);

            if (data == null)
                return HttpNotFound();

            db.Transactions.Remove(data);
            db.SaveChanges();

            return RedirectToAction("History");
        }

        // =========================
        // DROPDOWNS
        // =========================
        private void LoadDropdowns()
        {
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountNumber");
            ViewBag.TransactionTypeID = new SelectList(db.TransactionTypes, "TransactionTypeID", "TypeName");

            ViewBag.BeneficiaryID = new SelectList(
                db.Beneficiaries.Include("Account"),
                "BeneficiaryID",
                "NickName"
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}