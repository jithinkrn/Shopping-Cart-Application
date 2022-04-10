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

        public PurchaseController(DBContext dbContext)
        {

            this.dbContext = dbContext;

        }
        [Route("Purchases")]
        public IActionResult Index()
        {
            return View();
        }

    }
}
