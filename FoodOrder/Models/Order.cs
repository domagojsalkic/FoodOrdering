using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrder.Models
{
    public class Order
    {
        public Order()
        {
            TotalPrice = 0;
            IsPayed = false;
        }
        public int Id { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        
        [Column(TypeName = "money")]
        public decimal TotalPrice { get; set; }

        public bool IsPayed { get; set; }
    }
}
