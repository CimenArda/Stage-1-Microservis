using Microsoft.EntityFrameworkCore;
using ServiceB.Models;
using System;

namespace ServiceB.Context
{
    public class ServiceBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=ARDACIMEN\\SQLEXPRESS;initial Catalog =MicroservisDataSynchronizationEVENTBasedServiceB;integrated Security=true;");

        }
        public DbSet<Employee> Employees { get; set; }
    }
}
