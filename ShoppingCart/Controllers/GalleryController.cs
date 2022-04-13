using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace ShoppingCart.Controllers
{
    public class GalleryController : Controller
    {
        private DBContext dbContext;
        private const string UPLOAD_DIR = "Images";
        public GalleryController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index(string search, string sort)
        {
            //start of snippet
            Customer currentCustomer = new Customer();
            currentCustomer = CheckLoggedIn();

            ViewBag.CartContents = CountNumberOfItems();
            if (currentCustomer != null)
            {
                ViewBag.CurrentUserName = currentCustomer.FullName;
            }
            else
            {
                ViewBag.CurrentUserName = "Guest User";
            }
            //end of snippet of code

            if (search == null)
            {
                search = "";
            }

            List<Product> searchResult = dbContext.Products.Where(x => x.ProductName.Contains(search)).ToList();
            ViewData["searchResult"] = searchResult;
            ViewData["searchInput"] = search;

            if (sort == "asc")
            {
                ViewData["searchResult"] = ((List<Product>)ViewData["searchResult"]).OrderBy(x => x.Price).ToList();
            }
            else if (sort == "desc")
            {
                ViewData["searchResult"] = ((List<Product>)ViewData["searchResult"]).OrderByDescending(x => x.Price).ToList();
            }



            return View();
        }

        public IActionResult Product(Guid productId)
        {

            //start of snippet
            Customer currentCustomer = CheckLoggedIn();

            ViewBag.CartContents = CountNumberOfItems();
            if (currentCustomer != null)
            {
                ViewBag.CurrentUserName = currentCustomer.FullName;
            }
            else
            {
                ViewBag.CurrentUserName = "Guest User";
            }
            //end of snippet of code

            Product product = getProduct(productId);
            //retreive average rating and total count of ratings from DB
            Dictionary<string, int> productRating = getProductRating(productId);
            //Retrive List of reviews comments and respective customer from DB
            List<ReviewedCustomer> ReviewedCustomers = GetReviewComment(productId);

            //pass all retrieved info to the view
         
            Customer currentCustomer = CheckLoggedIn();

            ViewBag.currentCustomer = currentCustomer;
            ViewBag.product = product;
            ViewBag.productRating = productRating;
            ViewBag.reviewedCustomers = ReviewedCustomers;
            ViewBag.uploadDir = "../" + UPLOAD_DIR;
            return View();          
        }

        //Returns List of reviews comments and respective customer Name from DB
        //public IEnumerable<object> GetReviewComment(Guid productId)
        public List<ReviewedCustomer> GetReviewComment(Guid productId)

        {
            List<ReviewedCustomer> reviewedCustomers = new List<ReviewedCustomer>();
            var reviewComments =(from p in dbContext.ProductRatings 
                                where p.ProductId == productId && !string.IsNullOrEmpty(p.ReviewComment)
                                join c in dbContext.Customers on p.CustomerId equals c.Id
                                select new
                                {
                                    c.FullName,
                                    p.ReviewComment
                                })
                                .ToList();
            foreach (var item in reviewComments)
            {
                ReviewedCustomer reviewedCustomer = new ReviewedCustomer();
                reviewedCustomer.FullName = item.FullName;
                reviewedCustomer.ReviewComment = item.ReviewComment;
                reviewedCustomers.Add(reviewedCustomer);
            }
            return reviewedCustomers;
        }
        public Product getProduct(Guid productId)
        {
            Product product = dbContext.Products.Where(x =>
               x.Id == productId
           ).First();

            return product;
        }
        public IActionResult PassToCart([FromBody] ProdJson prodJson)
        {
            CartController cart = new CartController(dbContext);
            string ProductName = prodJson.ProductName;
            Customer curCustomer = CheckLoggedIn();

            //add to gust cart if cutomer is not logged in
            if (curCustomer == null)
            {
                cart.AddToCart(ProductName);               
            }
            //add to cutomers cart if cutomer is has logged in
            else
            {
                cart.AddToCart(ProductName, curCustomer);                
            }
            //return ok to Jason
            return Json(new { isOkay = true });
        }
        
        public Dictionary<string, int> getProductRating(Guid productId)
        {
            bool exist = dbContext.ProductRatings.Where(x => x.ProductId == productId).Any();
            var result = new Dictionary<string, int>();
            var avgRating = 0;
            var ratingCount = 0;
            if (exist)
            {
                avgRating = Convert.ToInt32(dbContext.ProductRatings.Where(x => x.ProductId == productId).Average
                              (x => x.Rating));

                ratingCount = dbContext.ProductRatings.Where(x => x.ProductId == productId).Count();
            }           
            result.Add("average", avgRating);
            result.Add("count", ratingCount);
            return result;

        }
        public Customer CheckLoggedIn()
        {
            Customer currentCustomer = new Customer();

            if (Request.Cookies["SessionId"] != null)
            {
                Guid sessionId = Guid.Parse(Request.Cookies["SessionId"]);
                Debug.WriteLine(sessionId.ToString());
                Session session = dbContext.Sessions.FirstOrDefault(x => x.Id == sessionId);

                if (session == null)
                {
                    currentCustomer = null;
                    return currentCustomer;
                }
                currentCustomer = dbContext.Customers.FirstOrDefault(x => x == session.Customer);
            }
            else
            {
                currentCustomer = null;
            }
            return currentCustomer;
        }


        //for data on top of navbar
        public Customer CheckLoggedIn()
        {
            Customer currentCustomer = new Customer();

            if (Request.Cookies["SessionId"] != null)
            {

                Guid sessionId = Guid.Parse(Request.Cookies["SessionId"]);
                Session session = dbContext.Sessions.FirstOrDefault(x => x.Id == sessionId);

                if (session == null)
                {
                    currentCustomer = null;
                    return currentCustomer;
                }

                currentCustomer = dbContext.Customers.FirstOrDefault(x => x == session.Customer);

            }
            else
            {
                currentCustomer = null;
            }
            return currentCustomer;
        }

        public int CountNumberOfItems()
        {
            int finalCount = 0;

            Customer currentCustomer = CheckLoggedIn();

            if (currentCustomer != null)
            {
                //get all items in the cart under the customer
                List<Cart> itemsInCart = dbContext.Carts.Where(x => x.CustomerId == currentCustomer.Id).ToList();

                //take the quantity of each item
                foreach (Cart item in itemsInCart)
                {
                    finalCount += item.OrderQty;
                }
            }
            else
            {
                //get all items in the cart under the customer
                List<GuestCart> itemsInCart = dbContext.GuestCarts.ToList();

                //take the quantity of each item
                foreach (GuestCart item in itemsInCart)
                {
                    finalCount += item.OrderQty;
                }
            }

            return finalCount;
        }

    }
}
