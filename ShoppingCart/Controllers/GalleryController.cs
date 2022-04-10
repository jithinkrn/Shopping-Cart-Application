using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //initial values on load

            //check who is logged in first. if no log in, currentCustomer = guest user
            Customer currentCustomer = dbContext.Customers.FirstOrDefault(x => x.FullName == "Tom Cruise");
            //initialize values of ViewBag.CartContents with CountNumberOfItems(currentCustomer), initialize ViewBag.CurrentUserName to currentCustomer
            ViewBag.CurrentUserName = currentCustomer.FullName;
            CartController JustForTheMethod = new CartController(dbContext);
            ViewBag.CartContents = JustForTheMethod.CountNumberOfItems(currentCustomer);


            return View();
        }

        public IActionResult Product(string prodSclicked)

        {
            //Start cookie session
            //string sessionId = System.Guid.NewGuid().ToString();
            //Response.Cookies.Append("sessionId", sessionId);

            //Delete cookies at logout
            //Response.Cookies.Delete("sessionId");

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
            Product product = getProduct(productId);

            ViewBag.product = product;
            return View();

        }
        public Product getProduct(Guid productId)
        {
            Product product = dbContext.Products.Where(x =>
               x.Id == productId
           ).First();

            return product;

        }

    }
}
