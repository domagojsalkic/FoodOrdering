using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FoodOrder.Models;

namespace FoodOrder.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Product { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<Check> Check { get; set; }

        public DbSet<Payment> Payment { get; set; }
    }
}
