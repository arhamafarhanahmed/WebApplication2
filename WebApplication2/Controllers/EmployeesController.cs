using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class EmployeesController : Controller
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

        // GET: Employees
        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.Branch);
            return View(employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Employee employee = db.Employees.Find(id);

            if (employee == null)
                return HttpNotFound();

            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.BranchID = new SelectList(
                db.Branches,
                "BranchID",
                "BranchCode"
            );

            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "EmployeeID,BranchID,FirstName,LastName,CNIC,Designation,Salary,Phone,Email,HireDate")]
        Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.BranchID = new SelectList(
                db.Branches,
                "BranchID",
                "BranchCode",
                employee.BranchID
            );

            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Employee employee = db.Employees.Find(id);

            if (employee == null)
                return HttpNotFound();

            ViewBag.BranchID = new SelectList(
                db.Branches,
                "BranchID",
                "BranchCode",
                employee.BranchID
            );

            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "EmployeeID,BranchID,FirstName,LastName,CNIC,Designation,Salary,Phone,Email,HireDate")]
        Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.BranchID = new SelectList(
                db.Branches,
                "BranchID",
                "BranchCode",
                employee.BranchID
            );

            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Employee employee = db.Employees.Find(id);

            if (employee == null)
                return HttpNotFound();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);

            if (employee != null)
            {
                db.Employees.Remove(employee);
                db.SaveChanges();
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
    } }
