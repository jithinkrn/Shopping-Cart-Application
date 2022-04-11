using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

                return RedirectToAction("Index", "Login");
            }

            // create a new session and tag to customer
            Session session = new Session()
            {
                Customer = customer
            }

            dbContext.Sessions.Add(session);
            dbContext.SaveChanges();

            // ask browser to save and send back these cookies next time
            Response.Cookies.Append("SessionId", session.Id.ToString());
            Response.Cookies.Append("UserName", customer.UserName);


            //coming from checkout stuff

            return RedirectToAction("Index", "Gallery");
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
                return RedirectToAction("Index", "Login");

            }
            else
            {
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

    }

}
