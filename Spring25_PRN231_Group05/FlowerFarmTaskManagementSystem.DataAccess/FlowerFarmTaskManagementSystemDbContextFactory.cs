using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FlowerFarmTaskManagementSystem.DataAccess
{
    public class FlowerFarmTaskManagementSystemDbContextFactory : IDesignTimeDbContextFactory<FlowerFarmTaskManagementSystemDbContext>
    {
        public FlowerFarmTaskManagementSystemDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FlowerFarmTaskManagementSystem.API")) 
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FlowerFarmTaskManagementSystemDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new FlowerFarmTaskManagementSystemDbContext(optionsBuilder.Options);
        }
    }
}
