using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class UpdateCart
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    //this is used only for the binding of json data
}
