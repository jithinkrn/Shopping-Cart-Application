﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{

    public class Customer
    {
        public Customer()
        {
            Id = new Guid();
            ShoppingCarts = new List<ShoppingCart>();
            Purchases = new List<Purchase>();
            ProductRatings = new List<ProductRating>();
        }
    
        //primary key
        public Guid Id  { get; set; }

        [Required]
        [MaxLength(25)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }
        [Required]
        [MaxLength(8)]
        public string Password { get; set; }

        //Customer has 1 to many relationship with ShoppingCart
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }

        //Customer has 1 to many relationship with Purchase
        public virtual ICollection<Purchase> Purchases { get; set; }

        //Customer has 1 to many relationship with ProductRating 
        public virtual ICollection<ProductRating> ProductRatings { get; set; }

    }
}