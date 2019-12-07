using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237_assignment6;

namespace cis237_assignment6.Controllers
{
    [Authorize] //Checks to see if the user is logged in and sends them to log in if not.
    public class BeveragesController : Controller
    {
        private BeverageContext db = new BeverageContext();

        // GET: Beverages
        public ActionResult Index()
        {
            //Setup a variable to hold the cars data
            DbSet<Beverage> CarsToFilter = db.Beverages;

            //Setup some strings to hold the data that might
            //be in the session. If there is nothing in the session
            //we can still use these variables as a default.
            string filterName = "";
            string filterPack = "";
            string filterMin = "";
            string filterMax = "";
            
            //Min and max default prices
            decimal min = 0;
            decimal max = 100;

            //Check to see if there is a value in the session,
            //and if there is, assign it to the variable that
            //we setup to hold the value.
            if (!String.IsNullOrWhiteSpace(
                (string)Session["s_name"]
                ))
            {
                filterName = (string)Session["s_name"];
            }
            if (!String.IsNullOrWhiteSpace(
                (string)Session["s_pack"]
                ))
            {
                filterPack = (string)Session["s_pack"];
            }
            if (!String.IsNullOrWhiteSpace(
                (string)Session["s_min"]
                ))
            {
                filterMin = (string)Session["s_min"];
                min = Decimal.Parse(filterMin);
            }
            if (!String.IsNullOrWhiteSpace(
                (string)Session["s_max"]
                ))
            {
                filterMax = (string)Session["s_max"];
                max = Decimal.Parse(filterMax);
            }

            //Pushes through a lambda expression that defines the new filter rules.

            IEnumerable<Beverage> filtered = CarsToFilter.Where(
                car => car.price >= min && car.price <= max &&
                car.name.Contains(filterName) && car.pack.Contains(filterPack)
                );

            //Convert the dataset to a list now that the query work
            //is done on it.  The view is expecting a list,
            //so we convert the dataset to a list.
            IList<Beverage> finalFiltered = filtered.ToList();

            //Place everything into the viewbag for transfer.
            ViewBag.filterName = filterName;
            ViewBag.filterPack = filterPack;
            ViewBag.filterMin = filterMin;
            ViewBag.filterMax = filterMax;

            //Return the view with the filtered selection of cars
            return View(finalFiltered);
        }

        // GET: Beverages/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // GET: Beverages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Beverages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Beverages.Add(beverage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(beverage);
        }

        // GET: Beverages/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: Beverages/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Method to take in the filter data and store it in
        //the session.  Will get used in the index method.
        public ActionResult Filter()
        {
            //Get the form data that we sent out of the request object.
            //The string that is used as a key to get the data matches
            //the name property of the form control.
            string name = Request.Form.Get("name");
            string pack = Request.Form.Get("pack");
            string min = Request.Form.Get("min");
            string max = Request.Form.Get("max");

            //Adding to session so other methods can access it.
            Session["s_name"] = name;
            Session["s_pack"] = pack;
            Session["s_min"] = min;
            Session["s_max"] = max;

            //Redirect to the index page
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
