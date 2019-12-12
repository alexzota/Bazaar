using GucciBazaar.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GucciBazaar.Controllers
{
    public class ProductAdditionRequestsController : Controller
    {
        private readonly ApplicationDbContext db = ApplicationDbContext.Create();

        [Route("ProductAdditionRequests/Index")]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Index()
        {
            List<ProductAdditionRequest> productRequests = new List<ProductAdditionRequest>();
            if (User.IsInRole("Administrator"))
            {
                productRequests = db.ProductAdditionRequests.OrderBy(m => m.CurrentState).ThenBy(m => m.UpdateDate).ToList();
            }
            else
            {
                var userId = User.Identity.GetUserId();
                productRequests = db.ProductAdditionRequests.Where(r => r.UserId == userId).OrderBy(m => m.CurrentState).ThenBy(m => m.UpdateDate).ToList();
            }


            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View(productRequests);
        }

        [Route("ProductAdditionRequests/Show/{Id}")]
        public ActionResult Show(int Id)
        {
            var productRequest = db.ProductAdditionRequests.Find(Id);

            return View(productRequest);
        }

        [HttpGet]
        [Route("ProductAdditionRequests/New")]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult New()
        {
            ProductAdditionRequest productRequest = new ProductAdditionRequest
            {
                Categories = GetAllCategories(),
                UserId = User.Identity.GetUserId()
            };

            return View(productRequest);
        }

        [HttpPost]
        [Route("ProductAdditionRequests/New")]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult New(ProductAdditionRequest productRequest)
        {
            productRequest.Categories = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    db.ProductAdditionRequests.Add(productRequest);
                    db.SaveChanges();
                    TempData["message"] = "A fost facuta cererea de adaugare a produsului!";
                    return RedirectToAction("Index", "ProductAdditionRequests");
                }
                else
                {
                    return View(productRequest);
                }
            }
            catch (Exception)
            {
                return View(productRequest);
            }
        }

        [HttpDelete]
        [Route("ProductAdditionRequests/Delete/{Id}")]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Delete(long Id)
        {
            var productRequest = db.ProductAdditionRequests.Find(Id);
            try
            {
                if (ModelState.IsValid)
                {
                    db.ProductAdditionRequests.Remove(productRequest);
                    db.SaveChanges();
                    TempData["message"] = "A fost stearsa cererea de adaugare a produsului!";
                    return RedirectToAction("Index", "ProductAdditionRequests");
                }
                else
                {
                    return View(productRequest);
                }
            }
            catch (Exception)
            {
                return View(productRequest);
            }
        }


        [Route("ProductAdditionRequest/Edit/{Id}")]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Edit(int Id)
        {
            ProductAdditionRequest productRequest = db.ProductAdditionRequests.Find(Id);
            productRequest.Categories = GetAllCategories();

            if (productRequest.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                return View(productRequest);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui request care nu va apartine!";
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        [Route("ProductAdditionRequest/Edit/{Id}")]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Edit(int Id, ProductAdditionRequest editedRequest)
        {
            editedRequest.Categories = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    ProductAdditionRequest productRequest = db.ProductAdditionRequests.Find(Id);
                    if (productRequest.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(productRequest))
                        {
                            productRequest.Title = editedRequest.Title;
                            productRequest.Description = editedRequest.Description;
                            productRequest.Price = editedRequest.Price;
                            productRequest.CategoryId = editedRequest.CategoryId;
                            productRequest.ImagePath = editedRequest.ImagePath;

                            db.SaveChanges();
                            TempData["message"] = "Cererea a fost modificata!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs care nu va apartine!";

                        return View(editedRequest);
                    }
                }
                else
                {
                    return View(editedRequest);
                }
            }
            catch (Exception)
            {
                return View(editedRequest);
            }
        }

        [HttpPost]
        [Route("ProductAdditionRequests/Approve/{Id}")]
        [Authorize(Roles = "Administrator")]
        public ActionResult Approve(long Id, string ApproverId)
        {
            var productRequest = db.ProductAdditionRequests.Find(Id);
            productRequest.ApproverId = ApproverId;
            productRequest.CurrentState = StateTypes.Approved;
            productRequest.UpdateDate = DateTime.UtcNow;

            var newProduct = new Product
            {
                UserId = productRequest.UserId,
                CategoryId = productRequest.CategoryId,
                Title = productRequest.Title,
                Description = productRequest.Description,
                Price = productRequest.Price,
                Rating = 0,
                ImagePath = productRequest.ImagePath,
            };

            try
            {
                if (ModelState.IsValid)
                {
                    db.Products.Add(newProduct);
                    db.SaveChanges();
                    TempData["message"] = "A fost aprobata cererea de adaugare a produsului!";
                    return RedirectToAction(actionName: $"Show/{newProduct.Id}", controllerName: "Product");
                }
                else
                {
                    return RedirectToAction(actionName: "Index", controllerName: "ProductAdditionRequests");
                }
            }
            catch (Exception)
            {
                return RedirectToAction(actionName: "Index", controllerName: "ProductAdditionRequests");
            }
        }

        [HttpPost]
        [Route("ProductAdditionRequests/Decline/{Id}")]
        [Authorize(Roles = "Administrator")]
        public ActionResult Decline(long Id, string ApproverId)
        {
            var productRequest = db.ProductAdditionRequests.Find(Id);
            productRequest.ApproverId = ApproverId;
            productRequest.CurrentState = StateTypes.Rejected;
            productRequest.UpdateDate = DateTime.UtcNow;
            try
            {
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    TempData["message"] = "A fost refuzata cererea de adaugare a produsului!";
                    return RedirectToAction("Index", "ProductAdditionRequests");
                }
                else
                {
                    return View(productRequest);
                }
            }
            catch (Exception)
            {
                return View(productRequest);
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