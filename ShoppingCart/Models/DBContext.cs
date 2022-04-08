﻿using System;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Models 
{ 
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
        }

        // maps to our tables in the database
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductRating> ProductRatings { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Session> Sesssions { get; set; }
    }
}
