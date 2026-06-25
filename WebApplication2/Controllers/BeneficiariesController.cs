using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class BeneficiariesController : Controller
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

        // GET: Beneficiaries
        public ActionResult Index()
        {
            var data = db.Beneficiaries
                         .Include(b => b.Customer)
                         .Include(b => b.Account)
                         .ToList();

            return View(data);
        }

        // GET: Details
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var ben = db.Beneficiaries
                        .Include(b => b.Customer)
                        .Include(b => b.Account)
                        .FirstOrDefault(b => b.BeneficiaryID == id);

            if (ben == null)
                return HttpNotFound();

            return View(ben);
        }

        // GET: Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerID");
            ViewBag.BeneficiaryAccountID = new SelectList(db.Accounts, "AccountID", "AccountNumber");

            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Beneficiary beneficiary)
        {
            if (ModelState.IsValid)
            {
                db.Beneficiaries.Add(beneficiary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerID", beneficiary.CustomerID);
            ViewBag.BeneficiaryAccountID = new SelectList(db.Accounts, "AccountID", "AccountNumber", beneficiary.BeneficiaryAccountID);

            return View(beneficiary);
        }

        // GET: Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var ben = db.Beneficiaries
                        .Include(b => b.Customer)
                        .Include(b => b.Account)
                        .FirstOrDefault(b => b.BeneficiaryID == id);

            if (ben == null)
                return HttpNotFound();

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerID", ben.CustomerID);
            ViewBag.BeneficiaryAccountID = new SelectList(db.Accounts, "AccountID", "AccountNumber", ben.BeneficiaryAccountID);

            return View(ben);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Beneficiary beneficiary)
        {
            if (ModelState.IsValid)
            {
                var existing = db.Beneficiaries.Find(beneficiary.BeneficiaryID);

                if (existing == null)
                    return HttpNotFound();

                existing.CustomerID = beneficiary.CustomerID;
                existing.BeneficiaryAccountID = beneficiary.BeneficiaryAccountID;
                existing.NickName = beneficiary.NickName;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(beneficiary);
        }

        // GET: Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var ben = db.Beneficiaries
                        .Include(b => b.Customer)
                        .Include(b => b.Account)
                        .FirstOrDefault(b => b.BeneficiaryID == id);

            if (ben == null)
                return HttpNotFound();

            return View(ben);
        }
        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var ben = db.Beneficiaries.Find(id);

                if (ben != null)
                {
                    db.Beneficiaries.Remove(ben);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error deleting beneficiary: " + ex.Message);

                var ben = db.Beneficiaries
                            .Include(b => b.Customer)
                            .Include(b => b.Account)
                            .FirstOrDefault(b => b.BeneficiaryID == id);

                return View("Delete", ben);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}