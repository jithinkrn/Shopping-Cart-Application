using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace ShoppingCart.Controllers
{
    public class LoginController : Controller
    {
        private DBContext dbContext;
      
        public LoginController(DBContext dbContext)
        {

            this.dbContext = dbContext;

        }
        public IActionResult Index()
        {
            if (Request.Cookies["SessionId"] != null)
            {
                Guid sessionId = Guid.Parse(Request.Cookies["SessionId"]);             

                Session session = dbContext.Sessions.FirstOrDefault(x =>
                    x.Id == sessionId
                );

                if (session == null)
                {
                    // someone has used an invalid Session ID (to fool us?); 
                    // route to Logout controller//
                    return RedirectToAction("Index", "Logout");
                }

                // valid Session ID; route to Gallery page
                return RedirectToAction("Index", "Gallery");
            }
            // no Session ID; show Login page
            return View();
        }
        public IActionResult Login(IFormCollection form)
        {
            string username = form["username"];
            string password = form["password"];
            string checkout = form["checkout"];

            HashAlgorithm sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(
                Encoding.UTF8.GetBytes(username + password));

            Customer customer = dbContext.Customers.FirstOrDefault(x =>
                 x.UserName == username &&
                 x.PassHash == hash
              );

            if (customer == null)
            {
                TempData["Message"] = "Username/Password Incorrect";
                ViewBag.CurrentUserName = "Guest User";
                ViewBag.CartContents = CountNumberOfItems();
                return RedirectToAction("Index", "Login");
            }

            // create a new session and tag to customer
            Session session = new Session()
            {
                Customer = customer
            };

            dbContext.Sessions.Add(session);
            dbContext.SaveChanges();

            // ask browser to save and send back these cookies next time
            Response.Cookies.Append("SessionId", session.Id.ToString());
            Response.Cookies.Append("UserName", customer.UserName);

            //snippet of code - existing customer
            if (dbContext.GuestCarts.ToList().Count() > 0) {
                TransferFromGuestToUserCart(customer);
            }


            ViewBag.CurrentUserName = customer.FullName;
            ViewBag.CartContents = CountNumberOfItems();
            //end snippet of code
            //coming from checkout stuff

            return RedirectToAction("Index", "Cart");
        }

        public IActionResult GuestLogin(IFormCollection form)
        {
            return RedirectToAction("Index", "Gallery");
        }

        /*Newuser Sign up*/


        public IActionResult Newusersignup(IFormCollection form)
        {
            return View();
        }
    
        public IActionResult Newuser(IFormCollection form)
        {
            string fullname = form["fullname"];
            string username = form["username"];
            string password = form["password"];

            HashAlgorithm sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(
                Encoding.UTF8.GetBytes(username + password));

            Customer customer = dbContext.Customers.FirstOrDefault(x =>
                x.UserName == username &&
                x.PassHash == hash
            );

            if (customer == null)
            {
                dbContext.Add(new Customer
                {
                    UserName = username,
                    FullName = fullname,
                    PassHash = hash
                });


                dbContext.SaveChanges();
                TempData["Message"] = "Registration Successful! Please Login as Member ";
                //snippet of code - no customer
                ViewBag.CurrentUserName = "Guest User";
                ViewBag.CartContents = CountNumberOfItems();
                //end snippet of code
                return RedirectToAction("Index", "Login");
            }
            else
            {
                //snippet of code - existing customer
                ViewBag.CurrentUserName = customer.FullName;
                ViewBag.CartContents = CountNumberOfItems();
                //end snippet of code
                TempData["Message"] = "User Already Exists";
                return RedirectToAction("Index", "Login");
            }

            /* dbContext.Add(new Customer
             {
                 UserName = username,
                 FullName = fullname,
                 PassHash = hash
             }); 

             dbContext.SaveChanges();
             return RedirectToAction("Index", "Login");*/

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

        public void TransferFromGuestToUserCart(Customer user)
        {

            Customer currentCustomer = user;

            Debug.WriteLine(currentCustomer.FullName);

            List<GuestCart> guestCart = dbContext.GuestCarts.ToList();
            List<Cart> userCart = dbContext.Carts.Where(x => x.CustomerId == user.Id).ToList();

            foreach (GuestCart item in guestCart)
            {
                Product newProd = dbContext.Products.FirstOrDefault(x => x.Id == item.ProductId);
                Cart itemInCart = dbContext.Carts.FirstOrDefault(x => x.ProductId == newProd.Id);
                Debug.WriteLine($"found {item} to shift from guest to cart");

                if (itemInCart == null)
                {
                    Debug.WriteLine($"didn't find {itemInCart} in cart, putting {item.OrderQty} in it");
                    //if not, create a new entry
                    dbContext.Carts.Add(new Cart
                    {
                        ProductId = newProd.Id,
                        CustomerId = currentCustomer.Id,
                        OrderQty = item.OrderQty
                    });
                }
                else
                {
                    Debug.WriteLine($"found {itemInCart} in cart, adding {item.OrderQty} in it");
                    //if found, add 1 to the product quantity
                    itemInCart.OrderQty += item.OrderQty;
                }

                Debug.WriteLine($"added {item.OrderQty} {newProd.ProductName} to userCart");

                dbContext.SaveChanges();

                dbContext.GuestCarts.Remove(item);
                Debug.WriteLine($"removed {item} from guestCart");

                dbContext.SaveChanges();
            }
        }

    }

}
