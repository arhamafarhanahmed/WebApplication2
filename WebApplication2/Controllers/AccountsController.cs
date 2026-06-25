using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class AccountsController : Controller
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
        // GET: Accounts
        public ActionResult Index()
        {
            var accounts = db.Accounts
                .Include(a => a.AccountType)
                .Include(a => a.Branch)
                .Include(a => a.Customer);

            return View(accounts.ToList());
        }

        // GET: Details
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var account = db.Accounts.Find(id);

            if (account == null)
                return HttpNotFound();

            return View(account);
        }

        // GET: Create
        public ActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Account account)
        {
            if (ModelState.IsValid)
            {
                account.OpenDate = DateTime.Now;

                db.Accounts.Add(account);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            LoadDropdowns(account);
            return View(account);
        }

        // GET: Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var account = db.Accounts.Find(id);

            if (account == null)
                return HttpNotFound();

            LoadDropdowns(account);
            return View(account);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Account account)
        {
            if (ModelState.IsValid)
            {
                var existing = db.Accounts.Find(account.AccountID);

                if (existing == null)
                    return HttpNotFound();

                existing.CustomerID = account.CustomerID;
                existing.BranchID = account.BranchID;
                existing.AccountTypeID = account.AccountTypeID;
                existing.AccountNumber = account.AccountNumber;
                existing.Balance = account.Balance;
                existing.Status = account.Status;
                existing.OpenDate = account.OpenDate;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            LoadDropdowns(account);
            return View(account);
        }

        // GET: Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var account = db.Accounts.Find(id);

            if (account == null)
                return HttpNotFound();

            return View(account);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var account = db.Accounts.Find(id);

            if (account != null)
            {
                db.Accounts.Remove(account);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // 🔥 Dropdown helper (IMPORTANT CLEAN FIX)
        private void LoadDropdowns(Account account = null)
        {
            ViewBag.AccountTypeID = new SelectList(db.AccountTypes, "AccountTypeID", "AccountTypeName", account?.AccountTypeID);
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchCode", account?.BranchID);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", account?.CustomerID);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}