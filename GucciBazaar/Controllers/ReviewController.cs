using GucciBazaar.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace GucciBazaar.Controllers
{
    [Authorize(Roles = "User,Editor,Administrator")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        [Route("Review/New")]
        public ActionResult New(Review review)
        {
            var product = db.Products.Where( m => m.Id == review.ProductId).ToList().FirstOrDefault();
            var reviewRating = review.Rating;

            var numberOfRatings = product.Reviews.Count();
            var sumOfRatings = product.Reviews.Sum(r => r.Rating);

            numberOfRatings++;
            sumOfRatings += reviewRating;

            product.Rating = sumOfRatings / numberOfRatings;

            try
            {
                if (ModelState.IsValid)
                {
                    db.Reviews.Add(review);
                    db.SaveChanges();
                    TempData["message"] = "A fost adaugat reviewul!";
                    return RedirectToAction($"Show/{product.Id}", "Product");
                }
                else
                {
                    return RedirectToAction($"Show/{product.Id}", "Product");
                }
            }
            catch (Exception)
            {
                return RedirectToAction($"Show/{product.Id}", "Product");
            }
        }

        [HttpDelete]
        [Route("Review/Delete/{Id}")]
        public ActionResult Delete(long Id)
        {
            var review = db.Reviews.Find(Id);
            var reviewRating = review.Rating;
            var productId = review.ProductId;
            Product product = db.Products.Where(m => m.Id == review.ProductId).ToList().FirstOrDefault();

            var numberOfRatings = product.Reviews.Count();
            var sumOfRatings = product.Reviews.Sum(r => r.Rating);

            numberOfRatings--;
            sumOfRatings -= reviewRating;

            product.Rating = sumOfRatings / numberOfRatings;

            try
            {
                if (ModelState.IsValid)
                {
                    db.Reviews.Remove(review);
                    db.SaveChanges();
                    TempData["message"] = "A fost adaugat reviewul!";
                    return RedirectToAction($"Show/{productId}", "Product");
                }
                else
                {
                    return RedirectToAction($"Show/{productId}", "Product");
                }
            }
            catch (Exception)
            {
                return RedirectToAction($"Show/{productId}", "Product");
            }
        }

        [HttpGet]
        [Route("Review/Edit/{Id}")]
        public ActionResult Edit(long Id)
        {
            Review review = db.Reviews.Find(Id);
            var productId = review.ProductId;
            Product product = db.Products.Find(productId);

            if (review.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                return View("~/Views/Product/EditReview.cshtml", review);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui review care nu va apartine!";
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        [Route("Review/Edit/{Id}")]
        public ActionResult Edit(long Id, Review newReview)
        {
            var review = db.Reviews.Find(Id);
            var reviewRating = review.Rating;
            var productId = review.ProductId;
            Product product = db.Products.Where(m => m.Id == review.ProductId).ToList().FirstOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    review.Rating = newReview.Rating;
                    review.Title = newReview.Title;
                    review.Description = newReview.Description;
                    review.UpdateDate = DateTime.UtcNow;

                    var numberOfRatings = product.Reviews.Count();
                    var sumOfRatings = product.Reviews.Sum(r => r.Rating);
                    product.Rating = sumOfRatings / numberOfRatings;

                    db.SaveChanges();
                    TempData["message"] = "A fost modificat reviewul!";
                    return RedirectToAction($"Show/{productId}", "Product");
                }
                else
                {
                    return RedirectToAction($"Show/{productId}", "Product");
                }
            }
            catch (Exception)
            {
                return RedirectToAction($"Show/{productId}", "Product");
            }
        }
    }
}