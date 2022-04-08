using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Controllers
{
    public class LogoutController : Controller
    {
        private DBContext dbContext;

        public LogoutController(DBContext dbContext)
        {
            
            this.dbContext = dbContext;
           
        }
        public IActionResult Index()
        {
            return View();
        }
        
        
    }
}
