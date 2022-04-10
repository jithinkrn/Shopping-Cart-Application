using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Controllers
{
    public class GalleryController : Controller
    {
        private DBContext dbContext;

        public GalleryController(DBContext dbContext)
        {
            //this comment is from gab reyes + jithin's comment ascadsfbva
            this.dbContext = dbContext;
            //TEst
        }

        public IActionResult Index()
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

            prodSclicked = ".NET Charts";

            if (prodSclicked == null)
            {
                return RedirectToAction("Index", "Gallery");
            }

            Product product = getProduct(prodSclicked);

            if (product == null)                
            {
                return RedirectToAction("Index", "Gallery");
            }
            ViewData["product"] = product;
            return View();
          
        }
        public Product getProduct(string productName)
        {                        
            Product product = dbContext.Products.Where(x =>
               x.ProductName == productName
           ).First();

            return product;

        }
        
    }
}
