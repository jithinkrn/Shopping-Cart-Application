using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Controllers
{
    public class CartController : Controller
    {
        private DBContext db;

        public CartController(DBContext dbContext)
        {

            this.db = dbContext;

        }
        public IActionResult Index()
        {

            //Seed an item for user
            Customer currentCustomer = db.Customers.FirstOrDefault(x => x.FullName == "Tom Cruise");

            Cart newItem = new Cart
            {
                ProductId = db.Products.FirstOrDefault(x => x.ProductName == ".NET ML").Id,
                CustomerId = currentCustomer.Id,
                OrderQty = 1
            };

            db.Carts.Add(newItem);

            db.SaveChanges();

            ViewBag.CurrentCustomer = currentCustomer;
            ViewBag.CustomerCart = db.Carts.Where(x => x.CustomerId == currentCustomer.Id).ToList();
            ViewBag.NewItem = db.Products.FirstOrDefault(x => x.Id == newItem.ProductId);

            return View();
        }

        //AddToCart (will be used in the gallery, the cart page and the results page)
        public void AddToCart(string productName) {
            //check for the current product in the database
            Product newProd = db.Products.FirstOrDefault(x => x.ProductName == productName);

            //check for the current customer who is using
            Customer currentCustomer = db.Customers.FirstOrDefault(x => x.FullName == "Tom Cruise");

            Cart itemInCart = db.Carts.FirstOrDefault(x => x.ProductId == newProd.Id && x.CustomerId == currentCustomer.Id);
            
            //query in the database whether the item already exists in the shopping cart of the customer
            if (itemInCart != null)
            {
                //if not, create a new entry
                db.Carts.Add(new Cart
                {
                    ProductId = newProd.Id,
                    CustomerId = currentCustomer.Id,
                    OrderQty = 1
                }) ;
            }
            else {
                //if found, add 1 to the product quantity
                itemInCart.OrderQty++;
            }

            db.SaveChanges();
        }

        //will be used in the cart page
        public void RemoveFromCart(string productName)
        {
            //check for the current product in the database
            Product newProd = db.Products.FirstOrDefault(x => x.ProductName == productName);

            //check for the current customer who is using
            Customer currentCustomer = db.Customers.FirstOrDefault(x => x.FullName == "Tom Cruise");

            //query for the item in the shoppingcarts db
            Cart itemInCart = db.Carts.FirstOrDefault(x => x.ProductId == newProd.Id && x.CustomerId == currentCustomer.Id);

            //query in the database whether there is only one more of the item
            if (itemInCart.OrderQty == 1)
            {
                //if yes, remove
                db.Carts.Remove(itemInCart);
            }
            else
            {
                //if not, subtract 1 to the product quantity
                itemInCart.OrderQty--;
            }

            db.SaveChanges();
        }

        public IActionResult Checkout()
        {
            //validate session

            //get currentcustomer
            Customer currentCustomer = db.Customers.FirstOrDefault(x => x.FullName == "Tom Cruise");

            //get all items in the cart under the customer
            List<Cart> itemsInCart = db.Carts.Where(x => x.CustomerId == currentCustomer.Id).ToList();

            //get all current purchases of customer
            //for display later in the checkout
            Dictionary<string, int> itemQty = new Dictionary<string, int>();
            Dictionary<string, double> itemPrice = new Dictionary<string, double>();

            //take the price of the entire list
            ViewBag.TotalPrice = CalculateTotalPrice();

            //transform all items into purchases
            foreach (Cart item in itemsInCart) {
                itemQty.Add(item.ProductId.ToString(), item.OrderQty);

                Purchase onHand = db.Purchases.FirstOrDefault(x => x.CustomerId == currentCustomer.Id && x.ProductId == item.ProductId);

                for (int i = item.OrderQty; i < 1; i--) {
                    //add to purchases list
                    if (onHand != null)
                    {
                        //if it is not in the purchases db, create new purchase, add activation code and set quantity to 1
                        onHand = new Purchase
                        {
                            ProductId = item.ProductId,
                            CustomerId = currentCustomer.Id,
                            PurchaseDate = DateTime.Now,
                            PurchaseQty = 1
                        };

                        db.Purchases.Add(onHand);
                    }
                    else
                    {
                        //if it is, add to purchase qty
                        onHand.PurchaseQty++;
                    }

                    db.SaveChanges();
                    //generate activation code
                    ActivationCode newCode = new ActivationCode
                    {
                        //put the attributes
                        ProductID = item.ProductId,
                        PurchaseID = onHand.Id
                    };

                    db.ActivationCodes.Add(newCode);

                    db.SaveChanges();

                }

                double currentPrice = CalculateTotalPrice();

                //remove from the ShoppingCarts database
                Cart findTheItem = db.Carts.FirstOrDefault(x => x.Id == item.Id);
                db.Carts.Remove(findTheItem);

                db.SaveChanges();
            }


            //empty the cart
            //send the details of the purchases to ViewBag (for View purposes)
            //add product details to the dictionary
            ViewBag.ItemQty = itemQty;
            ViewBag.ItemPrice = itemPrice;
            ViewBag.RecoItems = RecommendProducts();
            ViewBag.CurrentCustomer = currentCustomer;
            //send customer details to display

            return View();
        }

        public double CalculateTotalPrice() {
            double finalPrice = 0.0;

            //get current customer
            Customer currentCustomer = db.Customers.FirstOrDefault(x => x.FullName == "Tom Cruise");

            //get all items in the cart under the customer
            List<Cart> itemsInCart = db.Carts.Where(x => x.CustomerId == currentCustomer.Id).ToList();

            //take the price of each item
            foreach (Cart item in itemsInCart) {
                Product takePrice = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                //get the product price and multiply the quantity
                double priceToAdd = takePrice.Price * item.OrderQty;

                /*
                for any discount logic concerning the price, put them here
                */

                finalPrice += priceToAdd;
            }
            /*
            for any other discount logic concerning the price, put them here
            */

            return finalPrice;
        }

        //count number of items
        public int CountNumberOfItems() {
            int finalCount = 0;

            //find current customer
            Customer currentCustomer = db.Customers.FirstOrDefault(x => x.FullName == "Tom Cruise");

            //get all items in the cart under the customer
            List<Cart> itemsInCart = db.Carts.Where(x => x.CustomerId == currentCustomer.Id).ToList();

            //take the quantity of each item
            foreach (Cart item in itemsInCart)
            {
                finalCount += item.OrderQty;
            }
            
            return finalCount;
        }


        //compute how many reward points for the customer is needed
        public int ComputeRewardPoints(double price) {
            int rewardsTotal = (int)price / 10;
            return rewardsTotal;
        }

        public List<Product> RecommendProducts()
        {
            List<Product> listOfProducts = new List<Product>();

            while (listOfProducts.Count < 3)
            {
                Product randomProduct = FetchRandomProduct();
                if (!listOfProducts.Contains(randomProduct))
                {
                    listOfProducts.Add(randomProduct);
                }
            }

            return listOfProducts;
        }

        public Product FetchRandomProduct()
        {
            List<Product> listOfProducts = db.Products.ToList();
            Random rnd = new Random();

            Product randomProduct = listOfProducts[rnd.Next(listOfProducts.Count())];

            return randomProduct;
        }
    }
}
