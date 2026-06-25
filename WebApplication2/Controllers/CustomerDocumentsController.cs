using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class CustomerDocumentsController : Controller
    {
        private BMSEntities db = new BMSEntities();

        // =========================
        // INDEX
        // =========================

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
            var data = db.CustomerDocuments
                         .Include(c => c.Customer)
                         .ToList();

            return View(data);
        }

        // =========================
        // DETAILS
        // =========================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var doc = db.CustomerDocuments
                        .Include(c => c.Customer)
                        .FirstOrDefault(x => x.DocumentID == id);

            if (doc == null)
                return HttpNotFound();

            return View(doc);
        }

        // =========================
        // CREATE GET
        // =========================
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName");
            return View();
        }

        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerDocument model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", model.CustomerID);
                return View(model);
            }

            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);

                string folder = Server.MapPath("~/Uploads/CustomerDocuments/" + model.CustomerID);

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fullPath = Path.Combine(folder, fileName);
                file.SaveAs(fullPath);

                model.FileName = fileName;
                model.FilePath = "/Uploads/CustomerDocuments/" + model.CustomerID + "/" + fileName;
            }
            else
            {
                ModelState.AddModelError("", "Please select a file");
                ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", model.CustomerID);
                return View(model);
            }

            model.UploadDate = DateTime.Now;

            db.CustomerDocuments.Add(model);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // =========================
        // EDIT GET
        // =========================
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var doc = db.CustomerDocuments.Find(id);

            if (doc == null)
                return HttpNotFound();

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", doc.CustomerID);

            return View(doc);
        }

        // =========================
        // EDIT POST (FIXED)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerDocument model, HttpPostedFileBase file)
        {
            var existing = db.CustomerDocuments.Find(model.DocumentID);

            if (existing == null)
                return HttpNotFound();

            if (file != null && file.ContentLength > 0)
            {
                // delete old file
                if (!string.IsNullOrEmpty(existing.FilePath))
                {
                    string oldPath = Server.MapPath(existing.FilePath);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                string fileName = Path.GetFileName(file.FileName);
                string folder = Server.MapPath("~/Uploads/CustomerDocuments/" + model.CustomerID);

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fullPath = Path.Combine(folder, fileName);
                file.SaveAs(fullPath);

                existing.FileName = fileName;
                existing.FilePath = "/Uploads/CustomerDocuments/" + model.CustomerID + "/" + fileName;
            }

            existing.CustomerID = model.CustomerID;
            existing.DocumentType = model.DocumentType;
            existing.UploadDate = DateTime.Now;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // =========================
        // DELETE GET
        // =========================
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var doc = db.CustomerDocuments
                        .Include(c => c.Customer)
                        .FirstOrDefault(x => x.DocumentID == id);

            if (doc == null)
                return HttpNotFound();

            return View(doc);
        }

        // =========================
        // DELETE POST (FIXED)
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var doc = db.CustomerDocuments.Find(id);

            if (doc != null)
            {
                // delete physical file
                if (!string.IsNullOrEmpty(doc.FilePath))
                {
                    string path = Server.MapPath(doc.FilePath);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                db.CustomerDocuments.Remove(doc);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
        public ActionResult IndexList()
        {
            var data = db.CustomerDocuments
                         .Include(c => c.Customer)
                         .ToList();

            return View(data);
        }
    }
   

}