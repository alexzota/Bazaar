using GucciBazaar.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace GucciBazaar.Controllers
{
    [Authorize(Roles = "User,Editor,Administrator")]
    public class ShoppingCartController : Controller
    {
        readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        [Route("ShoppingCart/Index/{userId}")]
        public ActionResult Index(string userId)
        {
            var shoppingCart = db.ShoppingCarts.Where(m => m.UserId == userId).FirstOrDefault();
            ViewBag.TotalPrice = shoppingCart.Products.Sum(x => x.Product.Price*x.Quantity);
            
            return View(shoppingCart);
        }

        [HttpPut]
        [Route("ShoppingCart/AddProduct/Id")]
        public ActionResult AddProduct(long productId, string userId)
        {
            var product = db.Products.Find(productId);
            var cart = db.ShoppingCarts.Where(s => s.UserId == userId).FirstOrDefault();

            var cartProduct = new ShoppingCartProduct
            {
                ProductId = productId,
                Quantity = 1,
                Price = product.Price,
                ShoppingCartId = cart.Id
            };

            try
            {
                if (ModelState.IsValid)
                {
                    db.ShoppingCartProducts.Add(cartProduct);
                    db.SaveChanges();
                    TempData["message"] = "A fost facuta cererea de adaugare a produsului!";
                    return RedirectToAction("Index", new { userId });
                }
                else
                {
                    return RedirectToAction("Index", new { userId });
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { userId });
            }
        }

        [HttpPut]
        [Route("ShoppingCart/AddOne/{Id}")]
        public ActionResult AddOne(long Id)
        {
            var cartProduct = db.ShoppingCartProducts.Where(x => x.Id == Id).FirstOrDefault();
            cartProduct.Quantity++;
            cartProduct.Price += cartProduct.Product.Price;
            var userId = cartProduct.ShoppingCart.UserId;
            db.SaveChanges();

            return RedirectToAction("Index", new { userId });
        }

        [HttpPut]
        [Route("ShoppingCart/RemoveOne/{Id}")]
        public ActionResult RemoveOne(long Id)
        {
            var cartProduct = db.ShoppingCartProducts.Where(x => x.Id == Id).FirstOrDefault();
            var userId = cartProduct.ShoppingCart.UserId;
            if (cartProduct.Quantity == 1)
            {
                db.ShoppingCartProducts.Remove(cartProduct);
            }
            else
            {
                cartProduct.Quantity--;
                cartProduct.Price -= cartProduct.Product.Price;
            }

            db.SaveChanges();

            return RedirectToAction("Index", new { userId });
        }


        [HttpDelete]
        [Route("ShoppingCart/RemoveAll/{Id}")]
        public ActionResult RemoveAll(long Id)
        {
            var cartProduct = db.ShoppingCartProducts.Where(x => x.Id == Id).FirstOrDefault();

            var userId = cartProduct.ShoppingCart.UserId;

            db.ShoppingCartProducts.Remove(cartProduct);

            db.SaveChanges();

            return RedirectToAction("Index", new { userId });
        }

        [HttpGet]
        [Route("ShoppingCart/New/{userId}")]
        public ActionResult New(string userId)
        {
            ShoppingCart cart = new ShoppingCart
            {
                UserId = userId
            };
            return New(cart);
        }

        [HttpPost]
        [Route("ShoppingCart/New")]
        public ActionResult New(ShoppingCart shoppingCart)
        {
            db.ShoppingCarts.Add(shoppingCart);
            db.SaveChanges();

            return RedirectToAction("Index", "ShoppingCart", new { userId = shoppingCart .UserId});
        }
    }
}