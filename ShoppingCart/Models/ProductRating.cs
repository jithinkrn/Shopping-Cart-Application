using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ShoppingCart.Models
{
    public class ProductRating
    {
        public ProductRating()
        {
            Id = new Guid();
        }

        //PrimaryKey
        public Guid Id { get; set; }

        [MaxLength(1)]
        public int Rating { get; set; }
        public string ReviewComment { get; set; }

        //forien key to map to Customer
        public virtual Guid CustomerId { get; set; }

        //forien key to map to Product
        public virtual Guid ProductId { get; set; }




    }
}

