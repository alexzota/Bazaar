using GucciBazaar.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GucciBazaar.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Route("Product/Index")]
        public ActionResult Index(SortTypes sortBy = 0, string Name = "")
        {
            List<Product> products = db.Products.ToList();

            switch (sortBy)
            {
                case SortTypes.Rating:
                    products = products.OrderByDescending(x => x.Rating).ToList();
                    break;
                case SortTypes.Cheap:
                    products = products.OrderBy(x => x.Price).ToList();
                    break;
                case SortTypes.Expensive:
                    products = products.OrderByDescending(x => x.Price).ToList();
                    break;
                case SortTypes.Name:
                    products = db.Products.Where( x => x.Title.Contains(Name)).ToList();
                    break;
            }

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View(products);
        }

        [Route("Product/Show/{Id}")]
        public ActionResult Show(int Id)
        {
            var product = db.Products.Where(m => m.Id == Id).FirstOrDefault();

            return View(product);
        }

        [Route("Product/New")]
        [Authorize(Roles = "Administrator")]
        public ActionResult New()
        {
            Product product = new Product
            {
                UserId = User.Identity.GetUserId(),
                Categories = GetAllCategories()
            };

            return View(product);
        }

        [HttpPost]
        [Route("Product/New")]
        [Authorize(Roles = "Administrator")]
        public ActionResult New(Product product)
        {
            product.Categories = GetAllCategories();

            try
            {
                if (ModelState.IsValid)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    TempData["message"] = "A fost facuta cererea de adaugare a produsului!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(product);
                }
            }
            catch (Exception)
            {
                return View(product);
            }
        }

        [Route("Product/Edit/{Id}")]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Edit(long Id)
        {
            Product product = db.Products.Find(Id);
            product.Categories = GetAllCategories();

            if (product.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                return View(product);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs care nu va apartine!";
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        [Route("Product/Edit/{Id}")]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Edit(long Id, Product requestProduct)
        {
            requestProduct.Categories = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    Product product = db.Products.Find(Id);
                    if (product.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(product))
                        {
                            product.Title = requestProduct.Title;
                            product.Description = requestProduct.Description;
                            product.CategoryId = requestProduct.CategoryId;
                            product.Price = requestProduct.Price;
                            product.ImagePath = requestProduct.ImagePath;

                            db.SaveChanges();
                            TempData["message"] = "Produsul a fost modificat!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs care nu va apartine!";

                        return View(requestProduct);
                    }
                }
                else
                {
                    return View(requestProduct);
                }
            }
            catch (Exception)
            {
                return View(requestProduct);
            }
        }

        [HttpDelete]
        [Route("Product/Delete/{Id}")]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Delete(long Id)
        {
            Product article = db.Products.Find(Id);
            if (article.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                db.Products.Remove(article);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost sters!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un produs care nu va apartine!";
                return RedirectToAction("Index");
            }
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();

            var categories = db.Categories;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name.ToString()
                });
            }
            return selectList;
        }
    }
}
