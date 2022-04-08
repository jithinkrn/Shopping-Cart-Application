using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;


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
            //aaabbbcc
            
        }
               
        private void SeedCustomer()

        {


            dbContext.Add(new Customer
            {
                UserName = "Tom_Cruise01",
                FullName = "Tom Cruise",
                Password = "!@#$%FGs",
            });

            dbContext.Add(new Customer
            {
                UserName = "Brad_Pitt01",
                FullName = "Brad Pitt",
                Password = "@#$%ASDF",
            });

            dbContext.Add(new Customer
            {
                UserName = "Brad_Pitt02",
                FullName = "Brad Pitt",
                Password = "!@#$JKas",
            });
            dbContext.Add(new Customer
            {
                UserName = "Tom_Cruise02",
                FullName = "Tom Cruise",
                Password = "!@#%^&*E",
            });
            dbContext.Add(new Customer
            {
                UserName = "Al_Pacino01",
                FullName = "Al Pacino",
                Password = "!@#%^&*W",
            });

            dbContext.SaveChanges();
        }
        private void SeedProduct()
        {
            dbContext.Add(new Product
            {
               
                ProductName = ".NET Charts",
                Price = 99.00,
                Description = "Brings powerful charting capabilities to your .NET applications.",
                ImagePath = "\\ABC\\XYZ\\dotNETCharts.Jpg"

            });

            dbContext.Add(new Product
            {
               
                ProductName = ".NET Paypal",
                Price = 69.00,
                Description = "Integrate your .NET apps with paypal the easy way!",
                ImagePath = "\\ABC\\XYZ\\sample1.JPG"
            });

            dbContext.Add(new Product
            {
                ProductName = ".NET ML",
                Price = 299.00,
                Description = "Supercharged .NET machine learning libraries.",
                ImagePath = "\\ABC\\XYZ\\sample2.JPG"
            });

            dbContext.Add(new Product
            {
                ProductName = ".NET Analytics",
                Price = 299.00,
                Description = "Performs data mining and analytics easily in .NET.",
                ImagePath = "\\ABC\\XYZ\\sample3.JPG"
            });

            dbContext.Add(new Product
            {
                ProductName = ".NET Logger",
                Price = 299.00,
                Description = "Logs and aggregates events easily in your .NET apps.",
                ImagePath = "\\ABC\\XYZ\\sample4.JPG"
            });

            dbContext.Add(new Product
            {
                ProductName = ".NET Numerics",
                Price = 199.00,
                Description = "Powerful numerical method for your .NET simulations.",
                ImagePath = "\\ABC\\XYZ\\sample4.JPG"
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

    }
}

