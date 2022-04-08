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
        public IActionResult Index(string search, int sort, Guid productId)
        {
            if (search == null)
            {
                search = "";
            }

            List<Product> searchResult = dbContext.Products.Where(x => x.ProductName.Contains(search)).ToList();
            ViewData["searchResult"] = searchResult;
            ViewData["searchInput"] = search;

            if (sort == 1)
            {
                ViewData["searchResult"] = searchResult.OrderBy(x => x.Price).ToList();
            }
            else if (sort == 2)
            {
                ViewData["searchResult"] = searchResult.OrderByDescending(x => x.Price).ToList();
            }

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
