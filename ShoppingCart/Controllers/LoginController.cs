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
            {//session Id is converted to string as our Session controller has PK in string type//

                //  string sessionId = System.Guid.NewGuid().ToString();//
                string sessionId = (Request.Cookies["sessionId"]);
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

            HashAlgorithm sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(
                Encoding.UTF8.GetBytes(username + password));

            Customer customer = dbContext.Customers.FirstOrDefault(x =>
                 x.UserName == username &&
                 x.PassHash == hash
              );

            if (customer == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // create a new session and tag to user
            Session session = new Session()
            {
                Customer = customer
            };
            dbContext.Sessions.Add(session);
            dbContext.SaveChanges();

            // ask browser to save and send back these cookies next time
            Response.Cookies.Append("SessionId", session.Id.ToString());
            Response.Cookies.Append("Username", customer.UserName);

            return RedirectToAction("Index", "Gallery");
        }
    }
}

