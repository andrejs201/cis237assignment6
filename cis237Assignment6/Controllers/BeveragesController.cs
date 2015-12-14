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

            string filterName = "";
            string filterPack = "";
            string filterMinPrice = "";
            string filterMaxPrice = "";
            string filterActive = "";

            decimal minPrice = 0m;
            decimal maxPrice = 100m;

            if (Session["name"] != null && !String.IsNullOrWhiteSpace((string)Session["name"]))
            {
                filterName = (string)Session["name"];
            }

            if (Session["pack"] != null && !String.IsNullOrWhiteSpace((string)Session["pack"]))
            {
                filterPack = (string)Session["pack"];
            }

            if (Session["minPrice"] != null && !String.IsNullOrWhiteSpace((string)Session["minPrice"]))
            {
                filterMinPrice = (string)Session["minprice"];
                minPrice = Convert.ToDecimal(filterMinPrice);
            }

            if (Session["maxPrice"] != null && !String.IsNullOrWhiteSpace((string)Session["maxPrice"]))
            {
                filterMaxPrice = (string)Session["maxPrice"];
                maxPrice = Convert.ToDecimal(filterMaxPrice);
            }

            if (Session["active"] != null && !String.IsNullOrWhiteSpace((string)Session["active"]))
            {
                filterPack = (string)Session["active"];
            }

            IEnumerable<Beverage> filtered = BeveragesToSearch.Where(B => B.name.Contains(filterName) && B.pack.Contains(filterPack) && B.price >= minPrice && B.price <= maxPrice);

            IEnumerable<Beverage> finalFiltered = filtered.ToList();

            List<SelectListItem> list = new List<SelectListItem> { };
            list.Add(new SelectListItem { Text = "", Value = ""});
            list.Add(new SelectListItem { Text = "True", Value = "True"});
            list.Add(new SelectListItem { Text = "False", Value = "False"});

            IEnumerable<SelectListItem> dropBoxList = new List<SelectListItem> { new SelectListItem{ Text = "", Value = "" }, new SelectListItem{ Text = "True", Value="True"}, new SelectListItem{ Text = "False", Value = "False"}};
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
            //Get the form data that was sent out of the Request object. The string that is used as a key to get the data matches the name property of the form control.
            string name = Request.Form.Get("name");
            string pack = Request.Form.Get("pack");
            string minPrice = Request.Form.Get("minPrice");
            string maxPrice = Request.Form.Get("maxPrice");

            //Store the form data into the session so that it can be retrieved later on to filter the data.
            Session["name"] = name;
            Session["pack"] = pack;
            Session["minPrice"] = minPrice;
            Session["maxPrice"] = maxPrice;

            //Redirect the user to the index page. We will do the work of actually filtering the list in the index method.
            return RedirectToAction("Index");
        }
    }
}
