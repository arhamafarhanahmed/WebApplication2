using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class DebitCardsController : Controller
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
        // GET: DebitCards
        public ActionResult Index()
        {
            var debitCards = db.DebitCards.Include(d => d.Account).ToList();
            return View(debitCards);
        }

        // GET: DebitCards/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var card = db.DebitCards.Include(d => d.Account)
                                   .FirstOrDefault(x => x.CardID == id);

            if (card == null)
                return HttpNotFound();

            return View(card);
        }

        // GET: DebitCards/Create
        public ActionResult Create()
        {
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountNumber");
            return View();
        }

        // POST: DebitCards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DebitCard debitCard)
        {
            if (ModelState.IsValid)
            {
                // Default values (banking logic)
                debitCard.Status = "Active";

                // Expiry auto (5 years)
                if (debitCard.ExpiryDate == null)
                {
                    debitCard.ExpiryDate = DateTime.Now.AddYears(5);
                }

                db.DebitCards.Add(debitCard);
                db.SaveChanges();

                TempData["Success"] = "Debit card created successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountNumber", debitCard.AccountID);
            return View(debitCard);
        }

        // GET: DebitCards/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var card = db.DebitCards.Find(id);

            if (card == null)
                return HttpNotFound();

            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountNumber", card.AccountID);
            return View(card);
        }

        // POST: DebitCards/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DebitCard debitCard)
        {
            if (ModelState.IsValid)
            {
                db.Entry(debitCard).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success"] = "Debit card updated successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountNumber", debitCard.AccountID);
            return View(debitCard);
        }

        // GET: Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var card = db.DebitCards.Find(id);

            if (card == null)
                return HttpNotFound();

            return View(card);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var card = db.DebitCards.Find(id);

                if (card == null)
                {
                    TempData["Error"] = "Card not found.";
                    return RedirectToAction("Index");
                }

                db.DebitCards.Remove(card);
                db.SaveChanges();

                TempData["Success"] = "Debit card deleted successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Cannot delete this card due to linked records.";
            }

            return RedirectToAction("Index");
        }

        // ---------------------------
        // BLOCK CARD
        // ---------------------------
        public ActionResult Block(int id)
        {
            var card = db.DebitCards.Find(id);

            if (card != null)
            {
                card.Status = "Blocked";
                db.SaveChanges();

                TempData["Success"] = "Card blocked successfully.";
            }

            return RedirectToAction("Index");
        }

        // ---------------------------
        // UNBLOCK CARD
        // ---------------------------
        public ActionResult Unblock(int id)
        {
            var card = db.DebitCards.Find(id);

            if (card != null)
            {
                card.Status = "Active";
                db.SaveChanges();

                TempData["Success"] = "Card unblocked successfully.";
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}