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

        //forien key to map to Customer
        public virtual Guid CustomerID { get; set; }

        //forien key to map to Product
        public virtual Guid ProductID { get; set; }




    }
}

