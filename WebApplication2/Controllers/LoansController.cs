using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class LoansController : Controller
    {
        private BMSEntities db = new BMSEntities();

        // DASHBOARD

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserID"] == null)
            {
                filterContext.Result = RedirectToAction("Index", "Login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
        public ActionResult Index()
        {
            return View();
        }

        // LOAN HISTORY
        public ActionResult History()
        {
            var loans = db.Loans
                .Include(l => l.Customer)
                .Include(l => l.LoanType)
                .OrderByDescending(l => l.ApplyDate)
                .ToList();

            return View(loans);
        }

        // VIEW DETAILS
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var loan = db.Loans
                .Include(l => l.Customer)
                .Include(l => l.LoanType)
                .FirstOrDefault(l => l.LoanID == id);

            if (loan == null)
                return HttpNotFound();

            return View(loan);
        }

        // CREATE GET
        public ActionResult Create()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            var customer = db.Customers
                             .FirstOrDefault(c => c.UserID == userId);

            if (customer != null)
            {
                ViewBag.CustomerName =
                    customer.FirstName + " " + customer.LastName;
            }

            ViewBag.LoanTypeID = new SelectList(
                db.LoanTypes,
                "LoanTypeID",
                "LoanTypeName"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Loan loan)
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            var customer = db.Customers
                             .FirstOrDefault(c => c.UserID == userId);

            if (customer == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                loan.CustomerID = customer.CustomerID;
                loan.Status = "Pending";
                loan.ApplyDate = DateTime.Now;

                db.Loans.Add(loan);
                db.SaveChanges();

                return RedirectToAction("Loans", "Customer");
            }

            ViewBag.CustomerName =
                customer.FirstName + " " + customer.LastName;

            ViewBag.LoanTypeID = new SelectList(
                db.LoanTypes,
                "LoanTypeID",
                "LoanTypeName",
                loan.LoanTypeID
            );

            return View(loan);
        }
        // APPROVE LOAN
        public ActionResult Approve(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var loan = db.Loans.Find(id);

            if (loan == null)
                return HttpNotFound();

            loan.Status = "Approved";
            db.SaveChanges();

            return RedirectToAction("History");
        }

        // REJECT LOAN
        public ActionResult Reject(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var loan = db.Loans.Find(id);

            if (loan == null)
                return HttpNotFound();

            loan.Status = "Rejected";
            db.SaveChanges();

            return RedirectToAction("History");
        }

        // DELETE LOAN
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var loan = db.Loans.Find(id);

            if (loan == null)
                return HttpNotFound();

            db.Loans.Remove(loan);
            db.SaveChanges();

            return RedirectToAction("History");
        }

        // DROPDOWNS
        private void LoadDropdowns(int? selectedCustomer = null, int? selectedLoanType = null)
        {
            ViewBag.CustomerID = new SelectList(
                db.Customers.Select(c => new
                {
                    c.CustomerID,
                    FullName = c.FirstName + " " + c.LastName
                }),
                "CustomerID",
                "FullName",
                selectedCustomer
            );

            ViewBag.LoanTypeID = new SelectList(
                db.LoanTypes,
                "LoanTypeID",
                "LoanTypeName",
                selectedLoanType
            );
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