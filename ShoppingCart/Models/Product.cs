using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ShoppingCart.Models
{
    
    public class Product
    {
        public Product()
        {
            Id = new Guid();
           
            ShoppingCarts = new List<Cart>();
            ActivationCodes = new List<ActivationCode>();
            ProductRatings = new List<ProductRating>();
        }

        //Primary key
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
      
        public double Price { get; set; }

        [Required]
        [MaxLength(400)]
        public string Description { get; set; }
        [Required]
        [MaxLength(400)]
        public string ImageName { get; set; }

        //Reward points for discounts. cant be null. only get updated after purchase
        public int? RewardPoints { get; set; }

        //Products has 1 to many relationship with ShoppingCart 
        public virtual ICollection<Cart> ShoppingCarts { get; set; }

        //Products has 1 to many relationship with ActivationCode 
        public virtual ICollection<ActivationCode> ActivationCodes { get; set; }

        //Products has 1 to many relationship with ProductRating 
        public virtual ICollection<ProductRating> ProductRatings { get; set; }


    }
}
