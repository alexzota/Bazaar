using System.Web.Mvc;
using System.Linq;
using System;
using GucciBazaar.Models;

namespace GucciBazaar.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Category
        [Route("Category")]
        [Route("Category/Index")]
        public ActionResult Index()
        {
            var categories = (from category in db.Categories
                             orderby category.Name
                             select category).ToArray();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View(categories);
        }

        [Route("Category/Show/{Id}")]
        public ActionResult Show(int Id)
        {
            var category = db.Categories.Find(Id);

            return View(category);
        }

        [Route("Category/New")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Route("Category/New")]
        public ActionResult New(Category category)
        {
            try
            {
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["message"] = $"Categoria \"{category.Name}\" a fost adaugata cu succes";

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }

        [Route("Category/Edit/{Id}")]
        public ActionResult Edit(int Id)
        {
            var category = db.Categories.Find(Id);

            return View(category);
        }

        [HttpPut]
        [Route("Category/Edit/{Id}")]
        public ActionResult Edit(int Id, Category requestCategory)
        {
            try
            {
                var category = db.Categories.Find(Id);
                if (TryUpdateModel(category))
                {
                    category.Name = requestCategory.Name;
                    TempData["message"] = $"Categoria \"{category.Name}\" a fost modificata cu succes";
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpDelete]
        [Route("Category/Delete/{Id}")]
        public ActionResult Delete(int Id)
        {
            try
            {
                var category = db.Categories.Find(Id);
                db.Categories.Remove(category);
                TempData["message"] = $"Categoria \"{category.Name}\" a fost stearsa cu succes";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
    }
}