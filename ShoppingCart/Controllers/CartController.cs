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
        private readonly DBContext db;

        public CartController(DBContext dbContext)
        {

            this.db = dbContext;

        }

        public IActionResult Index()
        {
            //Seed an item for user
            Customer currentCustomer = db.Customers.FirstOrDefault(x => x.FullName == "Tom Cruise");

            Product newProd = FetchRandomProduct();

            AddToCart(newProd.ProductName, currentCustomer);

            db.SaveChanges();


            Dictionary<Product, int> countOfItems = new Dictionary<Product, int>();

            List<Cart> customerCart = db.Carts.Where(x => x.CustomerId == currentCustomer.Id).ToList();


            foreach (Cart item in customerCart) {
                Product newItem = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                countOfItems.Add(newItem, item.OrderQty);
            };

            ViewBag.CurrentCustomer = currentCustomer;
            ViewBag.CustomerCart = customerCart;
            ViewBag.CountOfItems = countOfItems;

            ViewBag.CartContents = CountNumberOfItems(currentCustomer);
            ViewBag.CurrentUserName = currentCustomer.FullName;

            return View();
        }

        //AddToCart (will be used in the gallery, the cart page and the results page)
        public void AddToCart(string productName, Customer currentUser) {
            //check for the current product in the database
            Product newProd = db.Products.FirstOrDefault(x => x.ProductName == productName);

            //check for the current customer who is using
            Customer currentCustomer = currentUser;

            Cart itemInCart = db.Carts.FirstOrDefault(x => x.ProductId == newProd.Id && x.CustomerId == currentCustomer.Id);
            
            //query in the database whether the item already exists in the shopping cart of the customer
            if (itemInCart == null)
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
        public void SubtractFromCart(string productName, Customer currentUser)
        {
            //check for the current product in the database
            Product newProd = db.Products.FirstOrDefault(x => x.ProductName == productName);

            //check for the current customer who is using
            Customer currentCustomer = currentUser;

            //query for the item in the shoppingcarts db
            Cart itemInCart = db.Carts.FirstOrDefault(x => x.ProductId == newProd.Id && x.CustomerId == currentCustomer.Id);


            if (itemInCart != null)
            {
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
            }

            db.SaveChanges();
        }
        public IActionResult Checkout()
        {
            //validate session
            //if not yet logged in, save all items? guestCart option?

            //get currentcustomer (will involve session)
            Customer currentCustomer = db.Customers.FirstOrDefault(x => x.FullName == "Tom Cruise");

            //call TransferFromGuestToUserCart(guestUser, currentCustomer); when logging in

            //get all items in the cart under the customer
            List<Cart> itemsInCart = db.Carts.Where(x => x.CustomerId == currentCustomer.Id).ToList();

            //get all current purchases of customer
            //for display later in the checkout
            Dictionary<string, int> itemQty = new Dictionary<string, int>();
            Dictionary<string, double> itemPrice = new Dictionary<string, double>();

            //take the price of the entire list
            ViewBag.TotalPrice = CalculateTotalPrice(currentCustomer);

            //transform all items into purchases
            foreach (Cart item in itemsInCart) {

                Purchase onHand = db.Purchases.FirstOrDefault(x => x.CustomerId == currentCustomer.Id && x.ProductId == item.ProductId);

                for (int i = item.OrderQty; i < 1; i--) {
                    //add to purchases list
                    if (onHand == null)
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

                //remove from the ShoppingCarts database
                Cart findTheItem = db.Carts.FirstOrDefault(x => x.Id == item.Id);
                Product findProduct = db.Products.FirstOrDefault(x => x.Id == findTheItem.ProductId);
                itemQty.Add(findProduct.ProductName, findTheItem.OrderQty);
                itemPrice.Add(findProduct.ProductName, findProduct.Price);

                db.Carts.Remove(findTheItem);

                db.SaveChanges();
            }


            //send the details of the purchases to ViewBag (for View purposes)
            //add product details to the dictionary
            ViewBag.ItemQty = itemQty;
            ViewBag.ItemPrice = itemPrice;
            ViewBag.ListOfRandomProds = RecommendProducts();
            ViewBag.CurrentCustomer = currentCustomer;
            //all IActionResult Methods will return this
            ViewBag.CartContents = CountNumberOfItems(currentCustomer);
            ViewBag.CurrentUserName = currentCustomer.FullName;
            //send customer details to display

            return View();
        }

        public double CalculateTotalPrice(Customer currentCustomer) {
            double finalPrice = 0.0;

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
        public int CountNumberOfItems(Customer currCustomer) {
            int finalCount = 0;

            //get all items in the cart under the customer
            List<Cart> itemsInCart = db.Carts.Where(x => x.CustomerId == currCustomer.Id).ToList();

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

        public void TransferFromGuestToUserCart(Customer guest, Customer user) {

            Customer currentCustomer = user;

            List<Cart> guestCart = db.Carts.Where(x => x.CustomerId == guest.Id).ToList();
            List<Cart> userCart = db.Carts.Where(x => x.CustomerId == user.Id).ToList();

            foreach (Cart item in guestCart) {
                Product newProd = db.Products.FirstOrDefault(x => x.Id == item.ProductId);

                for (int i = item.OrderQty; i < 0; i++) {
                    AddToCart(newProd.ProductName, user);
                }

                db.SaveChanges();

                db.Carts.Remove(item);

                db.SaveChanges();
            }
        }
    }
}
