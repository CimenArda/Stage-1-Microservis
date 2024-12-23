using Microsoft.EntityFrameworkCore;
using ServiceA.Models;


namespace ServiceA.Context
{
    public class ServiceAContext :DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=ARDACIMEN\\SQLEXPRESS;initial Catalog =MicroservisDataSynchronizationEVENTBasedServiceA;integrated Security=true;");

        }
        public DbSet<Person> Persons { get; set; }
    }
}
