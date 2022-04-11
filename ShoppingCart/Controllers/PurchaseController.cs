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
        [Route("Purchases")]
        public IActionResult Index()
        {
            // get the customId via session
            Customer currentCustomer = dbContext.Customers.FirstOrDefault(x => x.FullName == "Tom Cruise");

            // get purchase list via customid
            List<Purchase> purchases = dbContext.Purchases.Where(item => item.CustomerId.Equals(currentCustomer.Id)).ToList();

           
            Dictionary<Guid, Product> maps = new Dictionary<Guid, Product>();
            Dictionary<Guid, ActivationCode[]> activeCodeMap = new Dictionary<Guid, ActivationCode[]>();

            purchases.ForEach(purchase => {
                Product find = dbContext.Products.Where(item => item.Id.Equals(purchase.ProductId)).ToList().FirstOrDefault();

                ActivationCode[] codes = dbContext.ActivationCodes.Where(item => item.PurchaseID.Equals(purchase.Id)).ToArray();
                maps.Add(purchase.Id, find);
                activeCodeMap.Add(purchase.Id, codes);
            });


            // inject list to the view
            ViewData["purchaseList"] = purchases;
            ViewData["productMaps"] = maps;
            ViewData["activeCodeMap"] = activeCodeMap;
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
