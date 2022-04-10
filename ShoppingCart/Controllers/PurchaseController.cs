using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Controllers
{
    public class PurchaseController : Controller
    {
        private DBContext dbContext;
        private const string UPLOAD_DIR = "Images";
        public PurchaseController(DBContext dbContext)
        {

            this.dbContext = dbContext;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Review(string prodSclicked)
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
            ViewBag.product = product;
            ViewBag.uploadDir = "../" + UPLOAD_DIR;
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
