using Microsoft.EntityFrameworkCore;
using Order.API.Models;

namespace Order.API.Context
{
    public class OrderContext :DbContext
    {
        public OrderContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Order.API.Models.Order> Orders { get; set; }
        public DbSet<OrderItem> Items { get; set; } 


    }
}
