using Microsoft.EntityFrameworkCore;
using Service_A.Models;

namespace Service_A.Context
{
    public class Service_AContext :DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=ARDACIMEN\\SQLEXPRESS;initial Catalog =MicroservisDataSynchronizationAPIBasedServiceA;integrated Security=true;");

        }
        public DbSet<Person> Persons { get; set; }
    }
}
