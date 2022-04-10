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

        public GalleryController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index(string search, string sort)
    
        {
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
