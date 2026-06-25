using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class BranchesController : Controller
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
        // GET: Branches
        public ActionResult Index()
        {
            var branches = db.Branches.ToList();
            return View(branches);
        }

        // GET: Branches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var branch = db.Branches.FirstOrDefault(b => b.BranchID == id);

            if (branch == null)
                return HttpNotFound();

            return View(branch);
        }

        // GET: Branches/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Branches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Branch branch)
        {
            if (ModelState.IsValid)
            {
                db.Branches.Add(branch);
                db.SaveChanges();

                TempData["Success"] = "Branch created successfully.";
                return RedirectToAction("Index");
            }

            return View(branch);
        }

        // GET: Branches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var branch = db.Branches.FirstOrDefault(b => b.BranchID == id);

            if (branch == null)
                return HttpNotFound();

            return View(branch);
        }

        // POST: Branches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Branch branch)
        {
            if (ModelState.IsValid)
            {
                var existing = db.Branches.FirstOrDefault(b => b.BranchID == branch.BranchID);

                if (existing != null)
                {
                    existing.BranchCode = branch.BranchCode;
                    existing.BranchName = branch.BranchName;
                    existing.Address = branch.Address;
                    existing.City = branch.City;
                    existing.Phone = branch.Phone;

                    db.SaveChanges();

                    TempData["Success"] = "Branch updated successfully.";
                }

                return RedirectToAction("Index");
            }

            return View(branch);
        }

        // GET: Branches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var branch = db.Branches.FirstOrDefault(b => b.BranchID == id);

            if (branch == null)
                return HttpNotFound();

            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var branch = db.Branches.FirstOrDefault(b => b.BranchID == id);

                if (branch == null)
                {
                    TempData["Error"] = "Branch not found.";
                    return RedirectToAction("Index");
                }

                db.Branches.Remove(branch);
                db.SaveChanges();

                TempData["Success"] = "Branch deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete this branch because it is linked with employees, customers, accounts, or other records.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return RedirectToAction("Index");
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