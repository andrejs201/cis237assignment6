//Andrejs Tomsons

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237Assignment6;

namespace cis237Assignment6.Controllers
{
    [Authorize]
    public class BeveragesController : Controller
    {
        private BeverageATomsonsEntities db = new BeverageATomsonsEntities();

        // GET: /Beverages/
        public ActionResult Index()
        {
            DbSet<Beverage> BeveragesToSearch = db.Beverages;

            //Declare variables
            IEnumerable<Beverage> filtered = null;

            string filterName = "";
            string filterPack = "";
            string filterMinPrice = "";
            string filterMaxPrice = "";
            string filterActive = "";

            decimal minPrice = 0m;
            decimal maxPrice = 100m;

            //Set filterName
            if (Session["name"] != null && !String.IsNullOrWhiteSpace((string)Session["name"]))
            {
                filterName = (string)Session["name"];
            }

            //Set filterPack
            if (Session["pack"] != null && !String.IsNullOrWhiteSpace((string)Session["pack"]))
            {
                filterPack = (string)Session["pack"];
            }

            //Set filterMinPrice
            if (Session["minPrice"] != null && !String.IsNullOrWhiteSpace((string)Session["minPrice"]))
            {
                filterMinPrice = (string)Session["minprice"];
                minPrice = Convert.ToDecimal(filterMinPrice);
            }

            //Set filterMaxPrice
            if (Session["maxPrice"] != null && !String.IsNullOrWhiteSpace((string)Session["maxPrice"]))
            {
                filterMaxPrice = (string)Session["maxPrice"];
                maxPrice = Convert.ToDecimal(filterMaxPrice);
            }

            //Set filterActive
            if (Session["active"] != null && !String.IsNullOrWhiteSpace((string)Session["active"]))
            {
                filterActive = (string)Session["active"];
            }
            else
	        {
                filterActive = "";
	        }

            //Filter by name, pack and price
            if (filterActive == "")
            filtered = BeveragesToSearch.Where(B => B.name.Contains(filterName) && B.pack.Contains(filterPack) && B.price >= minPrice && B.price <= maxPrice);

            //Filter by name, pack, price and active true
            if (filterActive == "True")
            filtered = BeveragesToSearch.Where(B => B.name.Contains(filterName) && B.pack.Contains(filterPack) && B.price >= minPrice && B.price <= maxPrice && B.active == true);

            //Filter by name, pack, price and active false
            if (filterActive == "False")
            filtered = BeveragesToSearch.Where(B => B.name.Contains(filterName) && B.pack.Contains(filterPack) && B.price >= minPrice && B.price <= maxPrice && B.active == false);

            IEnumerable<Beverage> finalFiltered = filtered.ToList();

            //Create an array of SelectListItems for the dropbox with if statements to choose which one is selected.
            IEnumerable<SelectListItem> dropBoxList = new List<SelectListItem> { (filterActive == "") ? new SelectListItem { Text = "", Value = "", Selected = true } : new SelectListItem { Text = "", Value = "" }, 
                                                                                 (filterActive == "True") ? new SelectListItem { Text = "True", Value = "True", Selected = true } : new SelectListItem { Text = "True", Value = "True" }, 
                                                                                 (filterActive == "False") ? new SelectListItem { Text = "False", Value = "False", Selected = true } : new SelectListItem { Text = "False", Value = "False" }};

            //Put the needed variables in the ViewBag for later
            ViewBag.filterName = filterName;
            ViewBag.filterPack = filterPack;
            ViewBag.filterMinPrice = filterMinPrice;
            ViewBag.filterMaxPrice = filterMaxPrice;
            ViewBag.dropBoxList = dropBoxList;

            return View(finalFiltered);
        }

        // GET: /Beverages/Details/5
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

        // GET: /Beverages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Beverages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Beverages.Add(beverage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(beverage);
        }

        // GET: /Beverages/Edit/5
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

        // POST: /Beverages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: /Beverages/Delete/5
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

        // POST: /Beverages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
            db.SaveChanges();
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

        [HttpPost, ActionName("Filter")]
        [ValidateAntiForgeryToken]
        public ActionResult Filter()
        {
            //Store data from the form into the session
            Session["name"] = Request.Form.Get("name");
            Session["pack"] = Request.Form.Get("pack");
            Session["minPrice"] = Request.Form.Get("minPrice");
            Session["maxPrice"] = Request.Form.Get("maxPrice");
            Session["active"] = Request.Form.Get("active");

            return RedirectToAction("Index");
        }
    }
}
