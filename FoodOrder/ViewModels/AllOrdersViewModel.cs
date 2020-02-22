using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodOrder.Models;

namespace FoodOrder.ViewModels
{
    public class AllOrdersViewModel
    {
        public IEnumerable<Check> Checks { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
