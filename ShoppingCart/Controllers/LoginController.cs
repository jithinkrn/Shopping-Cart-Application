using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;

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
                
                string sessionId = System.Guid.NewGuid().ToString();
               sessionId = (Request.Cookies["sessionId"]);
                Session session = dbContext.Sessions.FirstOrDefault(x =>
                    x.Id == sessionId     
                );

                if (session == null)
                {
                    // someone has used an invalid Session ID (to fool us?); 
                    // route to Logout controller
                    return RedirectToAction("Index", "Logout");
                }

                // valid Session ID; route to Home page
                return RedirectToAction("Index", "Home");
            }

            // no Session ID; show Login page
            return View();
        }

    }
}
