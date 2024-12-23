using Microsoft.EntityFrameworkCore;
using Service_B.Models;
using System;

namespace Service_B.Context
{
    public class Service_BContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=ARDACIMEN\\SQLEXPRESS;initial Catalog =MicroservisDataSynchronizationAPIBasedServiceB;integrated Security=true;");

        }
        public DbSet<Employee> Employees { get; set; }
    }
}
