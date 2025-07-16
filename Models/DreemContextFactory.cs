using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Dreem.Models
{
    public class DreemContextFactory : IDesignTimeDbContextFactory<DreemContext>
    {
        public DreemContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DreemContext>();

            
            optionsBuilder.UseSqlServer("Server=localhost;Database=DreemDB;Trusted_Connection=True;TrustServerCertificate=True;");

            return new DreemContext(optionsBuilder.Options);
        }
    }
}
