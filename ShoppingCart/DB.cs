using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;
using System.Security.Cryptography;
using System.Text;


namespace ShoppingCart
{
    public class DB
    {           


        private DBContext dbContext;

        public DB(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Seed()
        {
            SeedCustomer();
            SeedProduct();
            SeedProductRating();
            SeedPurchase();

        }

      
        public void SeedCustomer()
        {
            // get a hash algorithm object
            HashAlgorithm sha = SHA256.Create();

            string[] usernames = { "Tom_Cruise01", "Brad_Pitt01", "Brad_Pitt02", "Tom_Cruise02", "Al_Pacino01"};
            string[] fullnames = { "Tom Cruise", "Brad Pitt", "Brad Pitt", "Tom Cruise", "Al Pacino"};
            // as our system's default, new users have their 
            // passwords set as "secret"
            string[] password = { "secret01", "secret02", "secret03", "secret04", "secret05"};

            for (int i = 0; i < usernames.Length; i++)
            {                           
                // we are concatenating (i.e. username + password) to generate
                // a password hash to store in the database
                string combo = usernames[i] + password[i];
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combo));

                // add customers
                dbContext.Add(new Customer
                {
                    UserName = usernames[i],
                    FullName = fullnames[i],
                    PassHash = hash,
                    RewardPoint = 0
                }); ;

                // commit our changes in the database
                dbContext.SaveChanges();
            }
        }
            
        private void SeedProduct()
        {
            dbContext.Add(new Product
            {
               
                ProductName = ".NET Charts",
                Price = 99.00,
                Description = "Brings powerful charting capabilities to your .NET applications.",
                ImageName = "dotNETCharts.jpg"

            });

            dbContext.Add(new Product
            {
               
                ProductName = ".NET Paypal",
                Price = 69.00,
                Description = "Integrate your .NET apps with paypal the easy way!",
                ImageName = "dotNETPaypal.jpg"
            });

            dbContext.Add(new Product
            {
                ProductName = ".NET ML",
                Price = 299.00,
                Description = "Supercharged .NET machine learning libraries.",
                ImageName = "dotNETML.jpg"
            });

            dbContext.Add(new Product
            {
                ProductName = ".NET Analytics",
                Price = 299.00,
                Description = "Performs data mining and analytics easily in .NET.",
                ImageName = "dotNETAnalytics.jpg"
            });

            dbContext.Add(new Product
            {
                ProductName = ".NET Logger",
                Price = 299.00,
                Description = "Logs and aggregates events easily in your .NET apps.",
                ImageName = "dotNETLogger.jpg"
            });

            dbContext.Add(new Product
            {
                ProductName = ".NET Numerics",
                Price = 199.00,
                Description = "Powerful numerical method for your .NET simulations.",
                ImageName = "dotNETNumerics.jpg"
            });

            dbContext.SaveChanges();

        }
        private void SeedProductRating()
        {

            Customer customer = dbContext.Customers.FirstOrDefault(x =>
                x.UserName == "Tom_Cruise01"
            );
            Product product = dbContext.Products.FirstOrDefault(x =>
                x.ProductName == ".NET Charts"
            );

            if (customer != null && product != null)
            {
                ProductRating productRating1 = new ProductRating
                {
                    Rating = 1
                };
                customer.ProductRatings.Add(productRating1);
                product.ProductRatings.Add(productRating1);                               
            }

            dbContext.SaveChanges();
        }

        public void SeedPurchase()
        {
            Customer customer = dbContext.Customers.FirstOrDefault(x =>

                        x.UserName == "Tom_Cruise01"
                    );

            Product product = dbContext.Products.FirstOrDefault(x =>
                x.ProductName == ".NET Charts"
            );

            if (customer != null && product != null)
            {

                //dbContext.Add(new ActivationCode
                //{
                  
                //});              

                Purchase purchase = new Purchase
                {
                    ProductId = product.Id,
                    PurchaseQty = 2,
                    PurchaseDate = new DateTime(2021, 11, 1, 9, 0, 0, DateTimeKind.Local)
                };
                customer.Purchases.Add(purchase);
                product.Purchases.Add(purchase);

            }
            dbContext.SaveChanges();
        }

        }
    }

