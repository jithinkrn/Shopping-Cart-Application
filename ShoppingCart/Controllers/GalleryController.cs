using Microsoft.AspNetCore.Mvc;
using System;
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
